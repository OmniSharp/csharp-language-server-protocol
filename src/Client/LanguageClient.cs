using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LanguageClient : JsonRpcServerBase, ILanguageClient
    {
        private readonly Connection _connection;
        private readonly ClientInfo _clientInfo;
        private readonly ILspClientReceiver _receiver;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        private readonly IHandlerCollection _collection;

        // private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        // private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnClientStartedDelegate> _startedDelegates;
        private readonly IResponseRouter _responseRouter;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IServiceProvider _serviceProvider;

        // private readonly ILanguageClientConfiguration _configuration;
        private readonly IEnumerable<ICapability> _capabilities;
        private readonly object _initializationOptions;
        private readonly IWorkspaceFoldersManager _workspaceFoldersManager;
        private readonly DocumentUri _rootUri;
        private readonly TraceValue _traceValue;
        private readonly IRegistrationManager _registrationManager;
        private readonly ClientCapabilities _clientCapabilities;
        private readonly IProgressManager _progressManager;
        private readonly IClientWorkDoneManager _workDoneManager;

        public static Task<ILanguageClient> From(Action<LanguageClientOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<ILanguageClient> From(LanguageClientOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<ILanguageClient> From(Action<LanguageClientOptions> optionsAction, CancellationToken token)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static ILanguageClient PreInit(Action<LanguageClientOptions> optionsAction)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<ILanguageClient> From(LanguageClientOptions options, CancellationToken token)
        {
            var server = (LanguageClient)PreInit(options);
            await server.Initialize(token);

            return server;
        }

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ILanguageClient PreInit(LanguageClientOptions options)
        {
            return new LanguageClient(options);
        }

        internal LanguageClient(LanguageClientOptions options) : base(options)
        {
            _capabilities = options.SupportedCapabilities;
            _clientCapabilities = options.ClientCapabilities;
            var services = options.Services;
            services.AddLogging(builder => options.LoggingBuilderAction(builder));
            options.RequestProcessIdentifier ??= (options.SupportsContentModified
                ? new RequestProcessIdentifier(RequestProcessType.Parallel)
                : new RequestProcessIdentifier(RequestProcessType.Serial));
            // services.AddSingleton<IOptionsMonitor<LoggerFilterOptions>, LanguageClientLoggerFilterOptions>();

            _clientInfo = options.ClientInfo;
            _receiver = options.Receiver;
            var serializer = options.Serializer;
            var supportedCapabilities = new SupportedCapabilities();
            _textDocumentIdentifiers = new TextDocumentIdentifiers();
            var collection = new SharedHandlerCollection(supportedCapabilities, _textDocumentIdentifiers);
            services.AddSingleton<IHandlersManager>(collection);
            _collection = collection;
            // _initializeDelegates = initializeDelegates;
            // _initializedDelegates = initializedDelegates;
            _startedDelegates = options.StartedDelegates;
            _rootUri = options.RootUri;
            _traceValue = options.TraceValue;
            _initializationOptions = options.InitializationOptions;

            services.AddSingleton<IOutputHandler>(_ =>
                new OutputHandler(options.Output, options.Serializer, options.Receiver.ShouldFilterOutput, _.GetService<ILogger<OutputHandler>>()));
            services.AddSingleton(_collection);
            services.AddSingleton(_textDocumentIdentifiers);
            services.AddSingleton(serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.IReceiver>(options.Receiver);
            services.AddSingleton(options.Receiver);
            services.AddSingleton<ILanguageClient>(this);
            services.AddSingleton<LspRequestRouter>();
            services.AddSingleton<IRequestRouter<ILspHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            services.AddSingleton<IProgressManager, ProgressManager>();
            services.AddSingleton(_ => _.GetRequiredService<IProgressManager>() as IJsonRpcHandler);
            services.AddSingleton<IClientWorkDoneManager, ClientWorkDoneManager>();
            services.AddSingleton(_ => _.GetRequiredService<IClientWorkDoneManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            services.AddSingleton<RegistrationManager>();
            services.AddSingleton<IRegistrationManager>(_ => _.GetRequiredService<RegistrationManager>());
            if (options.DynamicRegistration)
            {
                services.AddSingleton(_ => _.GetRequiredService<RegistrationManager>() as IJsonRpcHandler);
            }

            var workspaceFoldersManager = new WorkspaceFoldersManager(this);
            services.AddSingleton(workspaceFoldersManager);
            services.AddSingleton<IWorkspaceFoldersManager>(workspaceFoldersManager);
            if (options.WorkspaceFolders)
            {
                services.AddSingleton<IJsonRpcHandler>(workspaceFoldersManager);
            }

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            _serviceProvider = serviceProvider;
            collection.SetServiceProvider(_serviceProvider);

            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _progressManager = _serviceProvider.GetRequiredService<IProgressManager>();
            _workDoneManager = _serviceProvider.GetRequiredService<IClientWorkDoneManager>();
            _registrationManager = _serviceProvider.GetRequiredService<RegistrationManager>();
            _workspaceFoldersManager = _serviceProvider.GetRequiredService<IWorkspaceFoldersManager>();

            _connection = new Connection(
                options.Input,
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                options.Receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _responseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException ?? (e => { }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );

            // We need to at least create Window here in case any handler does loggin in their constructor
            TextDocument = new TextDocumentLanguageClient(this, _serviceProvider);
            Client = new ClientLanguageClient(this, _serviceProvider);
            General = new GeneralLanguageClient(this, _serviceProvider);
            Window = new WindowLanguageClient(this, _serviceProvider);
            Workspace = new WorkspaceLanguageClient(this, _serviceProvider);

            workspaceFoldersManager.Add(options.Folders);

            var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            var serviceIdentifiers = _serviceProvider.GetServices<ITextDocumentIdentifier>().ToArray();
            _disposable.Add(_textDocumentIdentifiers.Add(serviceIdentifiers));
            _disposable.Add(_collection.Add(serviceHandlers));
        }

        public ITextDocumentLanguageClient TextDocument { get; }
        public IClientLanguageClient Client { get; }
        public IGeneralLanguageClient General { get; }
        public IWindowLanguageClient Window { get; }
        public IWorkspaceLanguageClient Workspace { get; }
        public IProgressManager ProgressManager => _progressManager;
        public IClientWorkDoneManager WorkDoneManager => _workDoneManager;
        public IRegistrationManager RegistrationManager => _registrationManager;
        public IWorkspaceFoldersManager WorkspaceFoldersManager => _workspaceFoldersManager;

        public InitializeParams ClientSettings { get; private set; }
        public InitializeResult ServerSettings { get; private set; }

        public IServiceProvider Services => _serviceProvider;

        public async Task Initialize(CancellationToken token)
        {
            var @params = new InitializeParams {
                TraceValue = _traceValue,
                Capabilities = _clientCapabilities,
                ClientInfo = _clientInfo,
                RootUri = _rootUri,
                RootPath = _rootUri?.GetFileSystemPath(),
                WorkspaceFolders = new Container<WorkspaceFolder>(_workspaceFoldersManager.CurrentWorkspaceFolders),
                InitializationOptions = _initializationOptions
            };
            RegisterCapabilities(@params.Capabilities);
            _workDoneManager.Initialize(@params.Capabilities.Window);

            ClientSettings = @params;

            _connection.Open();
            var serverParams = await this.RequestLanguageProtocolInitialize(ClientSettings, token);
            _receiver.Initialized();

            ServerSettings = serverParams;
            if (_collection.ContainsHandler(typeof(IRegisterCapabilityHandler)))
                RegistrationManager.RegisterCapabilities(serverParams.Capabilities);

            // TODO: pull supported fields and add any static registrations to the registration manager
            this.SendLanguageProtocolInitialized(new InitializedParams());
        }

        private void RegisterCapabilities(ClientCapabilities capabilities)
        {
            capabilities.Window ??= new WindowClientCapabilities();
            capabilities.Window.WorkDoneProgress = _collection.ContainsHandler(typeof(IProgressHandler));

            capabilities.Workspace ??= new WorkspaceClientCapabilities();
            capabilities.Workspace.Configuration = _collection.ContainsHandler(typeof(IConfigurationHandler));
            capabilities.Workspace.Symbol = UseOrTryAndFindCapability(capabilities.Workspace.Symbol);
            capabilities.Workspace.ExecuteCommand = UseOrTryAndFindCapability(capabilities.Workspace.ExecuteCommand);
            capabilities.Workspace.ApplyEdit = _collection.ContainsHandler(typeof(IApplyWorkspaceEditHandler));
            capabilities.Workspace.WorkspaceEdit = UseOrTryAndFindCapability(capabilities.Workspace.WorkspaceEdit);
            capabilities.Workspace.WorkspaceFolders = _collection.ContainsHandler(typeof(IWorkspaceFoldersHandler));
            capabilities.Workspace.DidChangeConfiguration =
                UseOrTryAndFindCapability(capabilities.Workspace.DidChangeConfiguration);
            capabilities.Workspace.DidChangeWatchedFiles =
                UseOrTryAndFindCapability(capabilities.Workspace.DidChangeWatchedFiles);

            capabilities.TextDocument ??= new TextDocumentClientCapabilities();
            capabilities.TextDocument.Synchronization =
                UseOrTryAndFindCapability(capabilities.TextDocument.Synchronization);
            capabilities.TextDocument.Completion = UseOrTryAndFindCapability(capabilities.TextDocument.Completion);
            capabilities.TextDocument.Hover = UseOrTryAndFindCapability(capabilities.TextDocument.Hover);
            capabilities.TextDocument.SignatureHelp =
                UseOrTryAndFindCapability(capabilities.TextDocument.SignatureHelp);
            capabilities.TextDocument.References = UseOrTryAndFindCapability(capabilities.TextDocument.References);
            capabilities.TextDocument.DocumentHighlight =
                UseOrTryAndFindCapability(capabilities.TextDocument.DocumentHighlight);
            capabilities.TextDocument.DocumentSymbol =
                UseOrTryAndFindCapability(capabilities.TextDocument.DocumentSymbol);
            capabilities.TextDocument.Formatting = UseOrTryAndFindCapability(capabilities.TextDocument.Formatting);
            capabilities.TextDocument.RangeFormatting =
                UseOrTryAndFindCapability(capabilities.TextDocument.RangeFormatting);
            capabilities.TextDocument.OnTypeFormatting =
                UseOrTryAndFindCapability(capabilities.TextDocument.OnTypeFormatting);
            capabilities.TextDocument.Definition = UseOrTryAndFindCapability(capabilities.TextDocument.Definition);
            capabilities.TextDocument.Declaration = UseOrTryAndFindCapability(capabilities.TextDocument.Declaration);
            capabilities.TextDocument.CodeAction = UseOrTryAndFindCapability(capabilities.TextDocument.CodeAction);
            capabilities.TextDocument.CodeLens = UseOrTryAndFindCapability(capabilities.TextDocument.CodeLens);
            capabilities.TextDocument.DocumentLink = UseOrTryAndFindCapability(capabilities.TextDocument.DocumentLink);
            capabilities.TextDocument.Rename = UseOrTryAndFindCapability(capabilities.TextDocument.Rename);
            capabilities.TextDocument.TypeDefinition =
                UseOrTryAndFindCapability(capabilities.TextDocument.TypeDefinition);
            capabilities.TextDocument.Implementation =
                UseOrTryAndFindCapability(capabilities.TextDocument.Implementation);
            capabilities.TextDocument.ColorProvider =
                UseOrTryAndFindCapability(capabilities.TextDocument.ColorProvider);
            capabilities.TextDocument.FoldingRange = UseOrTryAndFindCapability(capabilities.TextDocument.FoldingRange);
            capabilities.TextDocument.SelectionRange =
                UseOrTryAndFindCapability(capabilities.TextDocument.SelectionRange);
            capabilities.TextDocument.PublishDiagnostics =
                UseOrTryAndFindCapability(capabilities.TextDocument.PublishDiagnostics);
#pragma warning disable 618
            capabilities.TextDocument.CallHierarchy =
                UseOrTryAndFindCapability(capabilities.TextDocument.CallHierarchy);
            capabilities.TextDocument.SemanticTokens =
                UseOrTryAndFindCapability(capabilities.TextDocument.SemanticTokens);
#pragma warning restore 618
        }

        public async Task Shutdown()
        {
            if (_connection.IsOpen)
            {
                await this.RequestShutdown();
                this.SendExit();
            }

            await _connection.StopAsync();
            _connection.Dispose();
        }

        private T UseOrTryAndFindCapability<T>(Supports<T> supports)
        {
            var value = supports.IsSupported
                ? supports.Value
                : _capabilities.OfType<T>().FirstOrDefault() ?? Activator.CreateInstance<T>();
            if (value is IDynamicCapability dynamicCapability)
            {
                dynamicCapability.DynamicRegistration = _collection.ContainsHandler(typeof(IRegisterCapabilityHandler));
            }

            return value;
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask();

        public void Dispose()
        {
            _connection?.Dispose();
            _disposable?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
        protected override IResponseRouter ResponseRouter => _responseRouter;
        protected override IHandlersManager HandlersManager => _collection;
        public IDisposable Register(Action<ILanguageClientRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeClientRegistry(_serviceProvider, manager, _textDocumentIdentifiers));
            return manager.GetDisposable();
        }
    }

    class LangaugeClientRegistry : InterimLanguageProtocolRegistry<ILanguageClientRegistry>, ILanguageClientRegistry
    {
        public LangaugeClientRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(serviceProvider, handlersManager, textDocumentIdentifiers)
        {
        }
    }
}
