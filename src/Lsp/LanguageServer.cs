using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using OmniSharp.Extensions.LanguageServer.Handlers;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer
{
    public delegate Task InitializeDelegate(InitializeParams request);

    public class LanguageServer : ILanguageServer, IInitializeHandler, IInitializedHandler, IDisposable, IAwaitableTermination
    {
        private readonly Connection _connection;
        private readonly LspRequestRouter _requestRouter;
        private readonly ShutdownHandler _shutdownHandler = new ShutdownHandler();
        private readonly List<InitializeDelegate> _initializeDelegates = new List<InitializeDelegate>();
        private readonly ExitHandler _exitHandler;
        private ClientVersion? _clientVersion;
        private readonly HandlerCollection _collection = new HandlerCollection();
        private readonly IResponseRouter _responseRouter;
        private readonly LspReciever _reciever;
        private readonly TaskCompletionSource<InitializeResult> _initializeComplete = new TaskCompletionSource<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public LanguageServer(Stream input, Stream output)
            : this(input, new OutputHandler(output), new LspReciever(), new RequestProcessIdentifier())
        {
        }

        internal LanguageServer(Stream input, IOutputHandler output, LspReciever reciever, IRequestProcessIdentifier requestProcessIdentifier)
        {
            _reciever = reciever;
            _requestRouter = new LspRequestRouter(_collection);
            _responseRouter = new ResponseRouter(output);
            _connection = new Connection(input, output, reciever, requestProcessIdentifier, _requestRouter, _responseRouter);

            _exitHandler = new ExitHandler(_shutdownHandler);

            _disposable.Add(
                AddHandlers(this, _shutdownHandler, _exitHandler, new CancelRequestHandler(_requestRouter))
            );
        }

        public InitializeParams Client { get; private set; }
        public InitializeResult Server { get; private set; }

        public IDisposable AddHandler(IJsonRpcHandler handler)
        {
            return AddHandler(handler);
        }

        public IDisposable AddHandlers(IEnumerable<IJsonRpcHandler> handlers)
        {
            return AddHandlers(handlers.ToArray());
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            var handlerDisposable = _collection.Add(handlers);

            return new ImmutableDisposable(
                handlerDisposable,
                new Disposable(() =>
                {
                    var foundItems = handlers
                    .SelectMany(handler => _collection
                        .Where(x => handler == x.Handler)
                        .Where(x => x.AllowsDynamicRegistration)
                        .Select(x => x.Registration)
                        .Where(x => x != null))
                    .ToArray();

                    Task.Run(() => this.UnregisterCapability(new UnregistrationParams()
                    {
                        Unregisterations = foundItems
                    }));
                }));
        }

        public async Task Initialize()
        {
            _connection.Open();

            await _initializeComplete.Task;

            // Small delay to let client respond
            await Task.Delay(100);

            await DynamicallyRegisterHandlers();
        }

        async Task<InitializeResult> IRequestHandler<InitializeParams, InitializeResult>.Handle(InitializeParams request, CancellationToken token)
        {
            Client = request;

            await Task.WhenAll(_initializeDelegates.Select(c => c(request)));

            _clientVersion = request.Capabilities.GetClientVersion();

            if (_clientVersion == ClientVersion.Lsp3)
            {
                // handle client capabilites
                if (request.Capabilities.TextDocument != null)
                {
                    ProcessCapabilties(request.Capabilities.TextDocument);
                }

                if (request.Capabilities.Workspace != null)
                {
                    ProcessCapabilties(request.Capabilities.Workspace);
                }
            }

            var textDocumentCapabilities = Client.Capabilities.TextDocument;
            var workspaceCapabilities = Client.Capabilities.Workspace;

            var serverCapabilities = new ServerCapabilities()
            {
                CodeActionProvider = HasHandler<ICodeActionHandler>(textDocumentCapabilities.CodeAction),
                CodeLensProvider = GetOptions<ICodeLensOptions, CodeLensOptions>(textDocumentCapabilities.CodeLens, CodeLensOptions.Of),
                CompletionProvider = GetOptions<ICompletionOptions, CompletionOptions>(textDocumentCapabilities.Completion, CompletionOptions.Of),
                DefinitionProvider = HasHandler<IDefinitionHandler>(textDocumentCapabilities.Definition),
                DocumentFormattingProvider = HasHandler<IDocumentFormattingHandler>(textDocumentCapabilities.Formatting),
                DocumentHighlightProvider = HasHandler<IDocumentHighlightHandler>(textDocumentCapabilities.DocumentHighlight),
                DocumentLinkProvider = GetOptions<IDocumentLinkOptions, DocumentLinkOptions>(textDocumentCapabilities.DocumentLink, DocumentLinkOptions.Of),
                DocumentOnTypeFormattingProvider = GetOptions<IDocumentOnTypeFormattingOptions, DocumentOnTypeFormattingOptions>(textDocumentCapabilities.OnTypeFormatting, DocumentOnTypeFormattingOptions.Of),
                DocumentRangeFormattingProvider = HasHandler<IDocumentRangeFormattingHandler>(textDocumentCapabilities.RangeFormatting),
                DocumentSymbolProvider = HasHandler<IDocumentSymbolHandler>(textDocumentCapabilities.DocumentSymbol),
                ExecuteCommandProvider = GetOptions<IExecuteCommandOptions, ExecuteCommandOptions>(workspaceCapabilities.ExecuteCommand, ExecuteCommandOptions.Of),
                HoverProvider = HasHandler<IHoverHandler>(textDocumentCapabilities.Hover),
                ReferencesProvider = HasHandler<IReferencesHandler>(textDocumentCapabilities.References),
                RenameProvider = HasHandler<IRenameHandler>(textDocumentCapabilities.Rename),
                SignatureHelpProvider = GetOptions<ISignatureHelpOptions, SignatureHelpOptions>(textDocumentCapabilities.SignatureHelp, SignatureHelpOptions.Of),
                WorkspaceSymbolProvider = HasHandler<IWorkspaceSymbolsHandler>(workspaceCapabilities.Symbol)
            };

            var textSyncHandler = _collection
                .Select(x => x.Handler)
                .OfType<ITextDocumentSyncHandler>()
                .FirstOrDefault();

            if (_clientVersion == ClientVersion.Lsp2)
            {
                serverCapabilities.TextDocumentSync = textSyncHandler?.Options.Change ?? TextDocumentSyncKind.None;
            }
            else
            {
                serverCapabilities.TextDocumentSync = textSyncHandler?.Options ?? new TextDocumentSyncOptions()
                {
                    Change = TextDocumentSyncKind.None,
                    OpenClose = false,
                    Save = new SaveOptions() { IncludeText = false },
                    WillSave = false,
                    WillSaveWaitUntil = false
                };
            }

            _reciever.Initialized();

            // TODO: Need a call back here
            // serverCapabilities.Experimental;

            var result = Server = new InitializeResult() { Capabilities = serverCapabilities };

            // TODO:
            if (_clientVersion == ClientVersion.Lsp2)
            {
                _initializeComplete.SetResult(result);
            }

            return result;
        }

        public LanguageServer OnInitialize(InitializeDelegate @delegate)
        {
            _initializeDelegates.Add(@delegate);
            return this;
        }

        public Task Handle()
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                _initializeComplete.SetResult(Server);
            }
            return Task.CompletedTask;
        }

        private bool HasHandler<T>(DynamicCapability capability)
        {
            return capability.DynamicRegistration ? false : _collection.Any(z => z.Handler is T);
        }

        private bool HasHandler<T>(Supports<DynamicCapability> capability)
        {
            if (!capability.IsSupported) return false;
            return HasHandler<T>(capability.Value);
        }

        private T GetOptions<O, T>(DynamicCapability capability, Func<O, T> action)
            where T : class
        {
            if (capability.DynamicRegistration) return null;

            return _collection
                .Select(x => x.Registration?.RegisterOptions is O cl ? action(cl) : null)
                .FirstOrDefault(x => x != null);
        }

        private T GetOptions<O, T>(Supports<DynamicCapability> capability, Func<O, T> action)
            where T : class
        {
            if (!capability.IsSupported) return null;
            return GetOptions<O, T>(capability.Value, action);
        }

        private void ProcessCapabilties(object instance)
        {
            var values = instance
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(x => x.CanRead)
                .Select(x => x.GetValue(instance))
                .OfType<ISupports>();

            foreach (var value in values)
            {
                foreach (var handler in _collection.Where(x => x.HasCapability && x.CapabilityType == value.ValueType))
                {
                    handler.SetCapability(value.Value);
                }
            }
        }

        private async Task DynamicallyRegisterHandlers()
        {
            var registrations = new List<Registration>();
            foreach (var handler in _collection.Where(x => x.AllowsDynamicRegistration))
            {
                registrations.Add(handler.Registration);
            }

            if (registrations.Count == 0)
                return; // No dynamic registrations supported by client.

            var @params = new RegistrationParams() { Registrations = registrations };

            await this.RegisterCapability(@params);
        }

        public event ShutdownEventHandler Shutdown
        {
            add => _shutdownHandler.Shutdown += value;
            remove => _shutdownHandler.Shutdown -= value;
        }

        public event ExitEventHandler Exit
        {
            add => _exitHandler.Exit += value;
            remove => _exitHandler.Exit -= value;
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            return _responseRouter.SendRequest<T, TResponse>(method, @params);
        }

        public Task SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }

        public Task WasShutDown => ((IAwaitableTermination)_shutdownHandler).WasShutDown;

        public void Dispose()
        {
            _connection?.Dispose();
            _disposable?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
    }
}
