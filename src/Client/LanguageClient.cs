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
    public partial class LanguageClient : ILanguageClient, IDisposable
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
        private readonly InitializeTrace _trace;
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
            var server = (LanguageClient) PreInit(options);
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

        internal LanguageClient(LanguageClientOptions options)
        {
            _capabilities = options.SupportedCapabilities;
            _clientCapabilities = options.ClientCapabilities;
            var services = options.Services;
            services.AddLogging(builder => options.LoggingBuilderAction(builder));
            // services.AddSingleton<IOptionsMonitor<LoggerFilterOptions>, LanguageClientLoggerFilterOptions>();

            _clientInfo = options.ClientInfo;
            _receiver = options.Receiver;
            var serializer = options.Serializer;
            var supportedCapabilities = new SupportedCapabilities();
            _textDocumentIdentifiers = new TextDocumentIdentifiers();
            var collection = new SharedHandlerCollection(supportedCapabilities, _textDocumentIdentifiers);
            _collection = collection;
            // _initializeDelegates = initializeDelegates;
            // _initializedDelegates = initializedDelegates;
            _startedDelegates = options.StartedDelegates;
            _rootUri = options.RootUri;
            _trace = options.Trace;
            _initializationOptions = options.InitializationOptions;


            var registrationManager = new RegistrationManager(serializer);
            _registrationManager = registrationManager;
            if (options.DynamicRegistration)
            {
                services.AddSingleton<IJsonRpcHandler>(registrationManager);
            }

            var workspaceFoldersManager = new WorkspaceFoldersManager(this);
            workspaceFoldersManager.Add(options.Folders);
            _workspaceFoldersManager = workspaceFoldersManager;
            if (options.WorkspaceFolders)
            {
                services.AddSingleton<IJsonRpcHandler>(workspaceFoldersManager);
            }

            services.AddSingleton<IOutputHandler>(_ =>
                new OutputHandler(options.Output, options.Serializer, _.GetService<ILogger<OutputHandler>>()));
            services.AddSingleton(_collection);
            services.AddSingleton(_textDocumentIdentifiers);
            services.AddSingleton(serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.IReceiver>(options.Receiver);
            services.AddSingleton<ILspClientReceiver>(options.Receiver);
            services.AddSingleton(new RequestRouterOptions() {
                MaximumRequestTimeout = options.MaximumRequestTimeout
            });


            foreach (var item in options.Handlers)
            {
                services.AddSingleton(item);
            }

            foreach (var item in options.TextDocumentIdentifiers)
            {
                services.AddSingleton(item);
            }

            foreach (var item in options.HandlerTypes)
            {
                services.AddSingleton(typeof(IJsonRpcHandler), item);
            }

            foreach (var item in options.TextDocumentIdentifierTypes)
            {
                services.AddSingleton(typeof(ITextDocumentIdentifier), item);
            }

            services.AddJsonRpcMediatR(options.HandlerAssemblies);
            services.AddSingleton<ILanguageClient>(this);
            services.AddSingleton<LspRequestRouter>();
            services.AddSingleton<IRequestRouter<ILspHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            var foundHandlers = services
                .Where(x => typeof(IJsonRpcHandler).IsAssignableFrom(x.ServiceType) &&
                            x.ServiceType != typeof(IJsonRpcHandler))
                .ToArray();

            // Handlers are created at the start and maintained as a singleton
            foreach (var handler in foundHandlers)
            {
                services.Remove(handler);

                if (handler.ImplementationFactory != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationFactory));
                else if (handler.ImplementationInstance != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationInstance));
                else
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationType));
            }

            services.AddSingleton<IProgressManager, ProgressManager>();
            services.AddSingleton<IJsonRpcHandler>(_ => _.GetRequiredService<IProgressManager>() as IJsonRpcHandler);
            services.AddSingleton<IClientWorkDoneManager, ClientWorkDoneManager>();
            services.AddSingleton<IJsonRpcHandler>(_ => _.GetRequiredService<IClientWorkDoneManager>() as IJsonRpcHandler);
            services.AddSingleton(_registrationManager);
            services.AddSingleton(_workspaceFoldersManager);

            _serviceProvider = services.BuildServiceProvider();
            collection.SetServiceProvider(_serviceProvider);

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<ILspHandlerDescriptor>>();
            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _progressManager = _serviceProvider.GetRequiredService<IProgressManager>();
            _workDoneManager = _serviceProvider.GetRequiredService<IClientWorkDoneManager>();
            _connection = new Connection(
                options.Input,
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                options.Receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _responseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnServerError,
                options.SupportsContentModified,
                options.Concurrency
            );

            // We need to at least create Window here in case any handler does loggin in their constructor
            TextDocument = new TextDocumentLanguageClient(this, _serviceProvider);
            Client = new ClientLanguageClient(this, _serviceProvider);
            General = new GeneralLanguageClient(this, _serviceProvider);
            Window = new WindowLanguageClient(this, _serviceProvider);
            Workspace = new WorkspaceLanguageClient(this, _serviceProvider);

            _disposable.Add(_collection.Add(new CancelRequestHandler<ILspHandlerDescriptor>(requestRouter)));

            var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            var serviceIdentifiers = _serviceProvider.GetServices<ITextDocumentIdentifier>().ToArray();
            _disposable.Add(_textDocumentIdentifiers.Add(serviceIdentifiers));
            _disposable.Add(_collection.Add(serviceHandlers));

            foreach (var (name, handler) in options.NamedHandlers)
            {
                _disposable.Add(_collection.Add(name, handler));
            }

            foreach (var (name, handlerFunc) in options.NamedServiceHandlers)
            {
                _disposable.Add(_collection.Add(name, handlerFunc(_serviceProvider)));
            }
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
        // public ILanguageClientConfiguration Configuration => _configuration;

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var textDocumentIdentifier in handlers)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }

            return cd;
        }

        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier
        {
            return _textDocumentIdentifiers.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
        }

        public async Task Initialize(CancellationToken token)
        {
            var @params = new InitializeParams {
                Trace = _trace,
                Capabilities = _clientCapabilities,
                ClientInfo = _clientInfo,
                RootUri = _rootUri,
                RootPath = _rootUri?.GetFileSystemPath(),
                WorkspaceFolders = new Container<WorkspaceFolder>(_workspaceFoldersManager.WorkspaceFolders.Items),
                InitializationOptions = _initializationOptions
            };
            RegisterCapabilities(@params.Capabilities);
            _workDoneManager.Initialize(@params.Capabilities.Window);

            ClientSettings = @params;

            _connection.Open();
            var serverParams = await this.RequestInitialize(ClientSettings, token);

            ServerSettings = serverParams;
            if (_collection.ContainsHandler(typeof(IRegisterCapabilityHandler)))
                RegistrationManager.RegisterCapabilities(serverParams.Capabilities);

            // TODO: pull supported fields and add any static registrations to the registration manager
            _receiver.Initialized();
            this.SendInitialized(new InitializedParams());
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

            _connection.Dispose();
        }

        private T UseOrTryAndFindCapability<T>(Supports<T> supports)
        {
            var value = supports.IsSupported
                ? supports.Value
                : _capabilities.OfType<T>().FirstOrDefault();
            if (value is IDynamicCapability dynamicCapability)
            {
                dynamicCapability.DynamicRegistration = _collection.ContainsHandler(typeof(IRegisterCapabilityHandler));
            }

            return value;
        }

        private IDisposable RegisterHandlers(LspHandlerDescriptorDisposable handlerDisposable, CancellationToken token)
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in handlerDisposable.Descriptors)
            {
                if (descriptor.AllowsDynamicRegistration)
                {
                    registrations.Add(new Registration() {
                        Id = descriptor.Id.ToString(),
                        Method = descriptor.Method,
                        RegisterOptions = descriptor.RegistrationOptions
                    });
                }

                if (descriptor.OnClientStartedDelegate != null)
                {
                    // Fire and forget to initialize the handler
                    _initializeComplete
                        .Select(result =>
                            Observable.FromAsync(() => descriptor.OnClientStartedDelegate(this, result, token)))
                        .Merge()
                        .Subscribe();
                }
            }

            return Disposable.Empty;
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest @params)
        {
            _responseRouter.SendNotification(@params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return _responseRouter.SendRequest(method);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest<T>(method, @params);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(@params, cancellationToken);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
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
        public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
