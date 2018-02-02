using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;

namespace OmniSharp.Extensions.LanguageServer.Server
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
        private readonly Serializer _serializer;
        private readonly TaskCompletionSource<InitializeResult> _initializeComplete = new TaskCompletionSource<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly HandlerMatcherCollection _handlerMactherCollection;

        public LanguageServer(
            Stream input,
            Stream output,
            ILoggerFactory loggerFactory)
            : this(input, output, loggerFactory, addDefaultLoggingProvider: true)
        {
        }

        public LanguageServer(
            Stream input,
            Stream output,
            ILoggerFactory loggerFactory,
            bool addDefaultLoggingProvider)
            : this(input, output, new LspReciever(), new RequestProcessIdentifier(), loggerFactory, new Serializer(), addDefaultLoggingProvider)
        {
        }

        internal LanguageServer(
            Stream input,
            Stream output,
            LspReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            ILoggerFactory loggerFactory,
            Serializer serializer,
            bool addDefaultLoggingProvider)
        {
            var outputHandler = new OutputHandler(output, serializer);

            if (addDefaultLoggingProvider)
                loggerFactory.AddProvider(new LanguageServerLoggerProvider(this));

            _reciever = reciever;
            _loggerFactory = loggerFactory;
            _serializer = serializer;
            _handlerMactherCollection = new HandlerMatcherCollection
            {
                new TextDocumentMatcher(_loggerFactory.CreateLogger<TextDocumentMatcher>(), _collection.TextDocumentSyncHandlers),
                new ExecuteCommandMatcher(_loggerFactory.CreateLogger<ExecuteCommandMatcher>())
            };

            _requestRouter = new LspRequestRouter(_collection, loggerFactory, _handlerMactherCollection, _serializer);
            _responseRouter = new ResponseRouter(outputHandler, _serializer);
            _connection = new Connection(input, outputHandler, reciever, requestProcessIdentifier, _requestRouter, _responseRouter, loggerFactory, serializer);

            _exitHandler = new ExitHandler(_shutdownHandler);

            _disposable.Add(
                AddHandlers(this, _shutdownHandler, _exitHandler, new CancelRequestHandler(_requestRouter))
            );
        }

        public InitializeParams Client { get; private set; }
        public InitializeResult Server { get; private set; }

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            var handlerDisposable = _collection.Add(method, handler);

            return new ImmutableDisposable(
                handlerDisposable,
                new Disposable(() => {
                    var foundItems = _collection
                        .Where(x => handler == x.Handler)
                        .Where(x => x.AllowsDynamicRegistration)
                        .Select(x => x.Registration)
                        .Where(x => x != null)
                        .ToArray();

                    Task.Run(() => this.UnregisterCapability(new UnregistrationParams() {
                        Unregisterations = foundItems
                    }));
                }));
        }

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

        public IDisposable AddHandlerMatcher(IHandlerMatcher handlerMatcher)
        {
            return _handlerMactherCollection.Add(handlerMatcher);
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
            _serializer.SetClientCapabilities(_clientVersion.Value, request.Capabilities);

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
                CodeActionProvider = ccp.HasStaticHandler(textDocumentCapabilities.CodeAction),
                CodeLensProvider = ccp.GetStaticOptions(textDocumentCapabilities.CodeLens).Get<ICodeLensOptions, CodeLensOptions>(CodeLensOptions.Of),
                CompletionProvider = ccp.GetStaticOptions(textDocumentCapabilities.Completion).Get<ICompletionOptions, CompletionOptions>(CompletionOptions.Of),
                DefinitionProvider = ccp.HasStaticHandler(textDocumentCapabilities.Definition),
                DocumentFormattingProvider = ccp.HasStaticHandler(textDocumentCapabilities.Formatting),
                DocumentHighlightProvider = ccp.HasStaticHandler(textDocumentCapabilities.DocumentHighlight),
                DocumentLinkProvider = ccp.GetStaticOptions(textDocumentCapabilities.DocumentLink).Get<IDocumentLinkOptions, DocumentLinkOptions>(DocumentLinkOptions.Of),
                DocumentOnTypeFormattingProvider = ccp.GetStaticOptions(textDocumentCapabilities.OnTypeFormatting).Get<IDocumentOnTypeFormattingOptions, DocumentOnTypeFormattingOptions>(DocumentOnTypeFormattingOptions.Of),
                DocumentRangeFormattingProvider = ccp.HasStaticHandler(textDocumentCapabilities.RangeFormatting),
                DocumentSymbolProvider = ccp.HasStaticHandler(textDocumentCapabilities.DocumentSymbol),
                ExecuteCommandProvider = ccp.GetStaticOptions(workspaceCapabilities.ExecuteCommand).Reduce<IExecuteCommandOptions, ExecuteCommandOptions>(ExecuteCommandOptions.Of),
                HoverProvider = ccp.HasStaticHandler(textDocumentCapabilities.Hover),
                ReferencesProvider = ccp.HasStaticHandler(textDocumentCapabilities.References),
                RenameProvider = ccp.HasStaticHandler(textDocumentCapabilities.Rename),
                SignatureHelpProvider = ccp.GetStaticOptions(textDocumentCapabilities.SignatureHelp).Get<ISignatureHelpOptions, SignatureHelpOptions>(SignatureHelpOptions.Of),
                WorkspaceSymbolProvider = ccp.HasStaticHandler(workspaceCapabilities.Symbol)
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
                        .Min<ITextDocumentSyncHandler, TextDocumentSyncKind>(z => z.Options.Change);
                }
                else
                {
                    serverCapabilities.TextDocumentSync = TextDocumentSyncKind.None;
                }
            }
            else
            {
                if (ccp.HasStaticHandler(textDocumentCapabilities.Synchronization))
                {
                    // TODO: Merge options
                    serverCapabilities.TextDocumentSync =
                        textSyncHandlers.FirstOrDefault()?.Options ?? new TextDocumentSyncOptions() {
                            Change = TextDocumentSyncKind.None,
                            OpenClose = false,
                            Save = new SaveOptions() { IncludeText = false },
                            WillSave = false,
                            WillSaveWaitUntil = false
                        };
                }
                else
                {
                    serverCapabilities.TextDocumentSync = TextDocumentSyncKind.None;
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

        public Task WasShutDown => _shutdownHandler.WasShutDown;
        public Task WaitForExit => _exitHandler.WaitForExit;

        public void Dispose()
        {
            _connection?.Dispose();
            _disposable?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
    }
}
