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
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.LanguageServer
{
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly TaskCompletionSource<InitializeResult> _initializeComplete = new TaskCompletionSource<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public LanguageServer(Stream input, Stream output, ILoggerFactory loggerFactory)
            : this(input, new OutputHandler(output), new LspReciever(), new RequestProcessIdentifier(), loggerFactory)
        {
        }

        internal LanguageServer(Stream input, IOutputHandler output, LspReciever reciever, IRequestProcessIdentifier requestProcessIdentifier, ILoggerFactory loggerFactory)
        {
            // TODO: This might not be the best
            loggerFactory.AddProvider(new LanguageServerLoggerProvider(this));

            _reciever = reciever;
            _loggerFactory = loggerFactory;
            _requestRouter = new LspRequestRouter(_collection, loggerFactory);
            _responseRouter = new ResponseRouter(output);
            _connection = new Connection(input, output, reciever, requestProcessIdentifier, _requestRouter, _responseRouter, loggerFactory);

            _exitHandler = new ExitHandler(_shutdownHandler);

            _disposable.Add(
                AddHandlers(this, _shutdownHandler, _exitHandler, new CancelRequestHandler(_requestRouter))
            );
        }

        public InitializeParams Client { get; private set; }
        public InitializeResult Server { get; private set; }

        public IDisposable AddHandler(IJsonRpcHandler handler)
        {
            return AddHandlers(handler);
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
                new Disposable(() => {
                    var foundItems = handlers
                    .SelectMany(handler => _collection
                        .Where(x => handler == x.Handler)
                        .Where(x => x.AllowsDynamicRegistration)
                        .Select(x => x.Registration)
                        .Where(x => x != null))
                    .ToArray();

                    Task.Run(() => this.UnregisterCapability(new UnregistrationParams() {
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

            var ccp = new ClientCapabilityProvider(_collection);

            var serverCapabilities = new ServerCapabilities() {
                CodeActionProvider = ccp.HasHandler(textDocumentCapabilities.CodeAction),
                CodeLensProvider = ccp.GetOptions(textDocumentCapabilities.CodeLens).Get<ICodeLensOptions, CodeLensOptions>(CodeLensOptions.Of),
                CompletionProvider = ccp.GetOptions(textDocumentCapabilities.Completion).Get<ICompletionOptions, CompletionOptions>(CompletionOptions.Of),
                DefinitionProvider = ccp.HasHandler(textDocumentCapabilities.Definition),
                DocumentFormattingProvider = ccp.HasHandler(textDocumentCapabilities.Formatting),
                DocumentHighlightProvider = ccp.HasHandler(textDocumentCapabilities.DocumentHighlight),
                DocumentLinkProvider = ccp.GetOptions(textDocumentCapabilities.DocumentLink).Get<IDocumentLinkOptions, DocumentLinkOptions>(DocumentLinkOptions.Of),
                DocumentOnTypeFormattingProvider = ccp.GetOptions(textDocumentCapabilities.OnTypeFormatting).Get<IDocumentOnTypeFormattingOptions, DocumentOnTypeFormattingOptions>(DocumentOnTypeFormattingOptions.Of),
                DocumentRangeFormattingProvider = ccp.HasHandler(textDocumentCapabilities.RangeFormatting),
                DocumentSymbolProvider = ccp.HasHandler(textDocumentCapabilities.DocumentSymbol),
                ExecuteCommandProvider = ccp.GetOptions(workspaceCapabilities.ExecuteCommand).Get<IExecuteCommandOptions, ExecuteCommandOptions>(ExecuteCommandOptions.Of),
                HoverProvider = ccp.HasHandler(textDocumentCapabilities.Hover),
                ReferencesProvider = ccp.HasHandler(textDocumentCapabilities.References),
                RenameProvider = ccp.HasHandler(textDocumentCapabilities.Rename),
                SignatureHelpProvider = ccp.GetOptions(textDocumentCapabilities.SignatureHelp).Get<ISignatureHelpOptions, SignatureHelpOptions>(SignatureHelpOptions.Of),
                WorkspaceSymbolProvider = ccp.HasHandler(workspaceCapabilities.Symbol)
            };

            var textSyncHandlers = _collection
                .Select(x => x.Handler)
                .OfType<ITextDocumentSyncHandler>()
                .ToArray();

            if (_clientVersion == ClientVersion.Lsp2)
            {
                if (textSyncHandlers.Any())
                {
                    serverCapabilities.TextDocumentSync = textSyncHandlers
                        .Where(x => x.Options.Change != TextDocumentSyncKind.None)
                        .Min(z => z.Options.Change);
                }
                else
                {
                    serverCapabilities.TextDocumentSync = TextDocumentSyncKind.None;
                }
            }
            else
            {
                if (ccp.HasHandler(textDocumentCapabilities.Synchronization))
                {
                    // TODO: Merge options
                    serverCapabilities.TextDocumentSync = textSyncHandlers.FirstOrDefault()?.Options ?? new TextDocumentSyncOptions() {
                        Change = TextDocumentSyncKind.None,
                        OpenClose = false,
                        Save = new SaveOptions() { IncludeText = false },
                        WillSave = false,
                        WillSaveWaitUntil = false
                    };
                }
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
            var registrations = _collection
                .Where(x => x.AllowsDynamicRegistration)
                .Select(handler => handler.Registration)
                .ToList();

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

