using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using System.Reactive.Disposables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server.Configuration;
using OmniSharp.Extensions.LanguageServer.Server.Logging;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public partial class LanguageServer : JsonRpcServerBase, ILanguageServer, ILanguageProtocolInitializeHandler, ILanguageProtocolInitializedHandler, IAwaitableTermination,
        IDisposable
    {
        private readonly Connection _connection;
        private ClientVersion? _clientVersion;
        private readonly ServerInfo _serverInfo;
        private readonly IServerWorkDoneManager _serverWorkDoneManager;
        private readonly ILspServerReceiver _serverReceiver;
        private readonly ISerializer _serializer;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnServerStartedDelegate> _startedDelegates;
        private readonly IResponseRouter _responseRouter;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IServiceProvider _serviceProvider;
        private readonly SupportedCapabilities _supportedCapabilities;
        private Task _initializingTask;

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<LanguageServer> From(LanguageServerOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, CancellationToken token)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static LanguageServer PreInit(Action<LanguageServerOptions> optionsAction)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<LanguageServer> From(LanguageServerOptions options, CancellationToken token)
        {
            var server = (LanguageServer) PreInit(options);
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
        public static LanguageServer PreInit(LanguageServerOptions options)
        {
            return new LanguageServer(options);
        }

        internal LanguageServer(LanguageServerOptions options) : base(options)
        {
            var services = options.Services;
            var configurationProvider = new DidChangeConfigurationProvider(this, options.ConfigurationBuilderAction);
            services.AddSingleton<IJsonRpcHandler>(configurationProvider);
            options.RequestProcessIdentifier ??= (options.SupportsContentModified
                ? new RequestProcessIdentifier(RequestProcessType.Parallel)
                : new RequestProcessIdentifier(RequestProcessType.Serial));

            services.AddSingleton<IConfiguration>(configurationProvider);
            services.AddSingleton(Configuration = configurationProvider);

            services.AddLogging(builder => options.LoggingBuilderAction(builder));
            services.AddSingleton<IOptionsMonitor<LoggerFilterOptions>, LanguageServerLoggerFilterOptions>();

            _serverInfo = options.ServerInfo;
            _serverReceiver = options.Receiver;
            _serializer = options.Serializer;
            _supportedCapabilities = new SupportedCapabilities();
            _textDocumentIdentifiers = new TextDocumentIdentifiers();
            var collection = new SharedHandlerCollection(_supportedCapabilities, _textDocumentIdentifiers);
            services.AddSingleton<IHandlersManager>(collection);
            _collection = collection;
            _initializeDelegates = options.InitializeDelegates;
            _initializedDelegates = options.InitializedDelegates;
            _startedDelegates = options.StartedDelegates;

            services.AddSingleton<IOutputHandler>(_ => new OutputHandler(
                options.Output,
                options.Serializer,
                options.Receiver.ShouldFilterOutput,
                _.GetService<ILogger<OutputHandler>>())
            );
            services.AddSingleton(_collection);
            services.AddSingleton(_textDocumentIdentifiers);
            services.AddSingleton(_serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(_serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.IReceiver>(options.Receiver);
            services.AddSingleton<ILspServerReceiver>(options.Receiver);

            services.AddTransient<IHandlerMatcher, TextDocumentMatcher>();
            services.AddSingleton<ILanguageServer>(this);
            services.AddTransient<IHandlerMatcher, ExecuteCommandMatcher>();
            services.AddTransient<IHandlerMatcher, ResolveCommandMatcher>();
            services.AddSingleton<LspRequestRouter>();
            services.AddSingleton<IRequestRouter<ILspHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResolveCommandPipeline<,>));

            services.AddSingleton<IProgressManager, ProgressManager>();
            services.AddSingleton<IJsonRpcHandler>(_ => _.GetRequiredService<IProgressManager>() as IJsonRpcHandler);
            services.AddSingleton<IServerWorkDoneManager, ServerWorkDoneManager>();
            services.AddSingleton<IJsonRpcHandler>(_ => _.GetRequiredService<IServerWorkDoneManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            _serviceProvider = serviceProvider;
            collection.SetServiceProvider(_serviceProvider);

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<ILspHandlerDescriptor>>();
            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            ProgressManager = _serviceProvider.GetRequiredService<IProgressManager>();
            _serverWorkDoneManager = _serviceProvider.GetRequiredService<IServerWorkDoneManager>();
            _connection = new Connection(
                options.Input,
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                options.Receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _responseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException ?? (e => { ForcefulShutdown(); }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );

            // We need to at least create Window here in case any handler does loggin in their constructor
            TextDocument = new TextDocumentLanguageServer(this, _serviceProvider);
            Client = new ClientLanguageServer(this, _serviceProvider);
            General = new GeneralLanguageServer(this, _serviceProvider);
            Window = new WindowLanguageServer(this, _serviceProvider);
            Workspace = new WorkspaceLanguageServer(this, _serviceProvider);

            _disposable.Add(_collection.Add(this));

            {
                var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
                var serviceIdentifiers = _serviceProvider.GetServices<ITextDocumentIdentifier>().ToArray();
                _disposable.Add(_textDocumentIdentifiers.Add(serviceIdentifiers));
                _disposable.Add(_collection.Add(serviceHandlers));
                options.AddLinks(_collection);
            }
        }


        public ITextDocumentLanguageServer TextDocument { get; }
        public IClientLanguageServer Client { get; }
        public IGeneralLanguageServer General { get; }
        public IWindowLanguageServer Window { get; }
        public IWorkspaceLanguageServer Workspace { get; }

        public InitializeParams ClientSettings { get; private set; }
        public InitializeResult ServerSettings { get; private set; }

        public IServiceProvider Services => _serviceProvider;
        public IProgressManager ProgressManager { get; }

        public IServerWorkDoneManager WorkDoneManager => _serverWorkDoneManager;
        public ILanguageServerConfiguration Configuration { get; }

        public async Task Initialize(CancellationToken token)
        {
            if (_initializingTask != null)
            {
                try
                {
                    await _initializingTask;
                }
                catch
                {
                    // Swallow exceptions because the original initialization task will report errors if it fails (don't want to doubly report).
                }

                return;
            }

            _connection.Open();
            try
            {
                _initializingTask = _initializeComplete
                    .Select(result => _startedDelegates.Select(@delegate =>
                            Observable.FromAsync(() => @delegate(this, result, token))
                        )
                        .ToObservable()
                        .Merge()
                        .Select(z => result)
                    )
                    .Merge()
                    .LastOrDefaultAsync()
                    .ToTask(token);
                await _initializingTask;
            }
            catch (TaskCanceledException e)
            {
                _initializeComplete.OnError(e);
                throw;
            }
            catch (Exception e)
            {
                _initializeComplete.OnError(e);
                throw;
            }
        }

        async Task<InitializeResult> IRequestHandler<InitializeParams, InitializeResult>.Handle(
            InitializeParams request, CancellationToken token)
        {
            ClientSettings = request;

            if (request.TraceValue == TraceValue.Verbose)
            {
                var loggerSettings = _serviceProvider.GetService<LanguageServerLoggerSettings>();

                if (loggerSettings?.MinimumLogLevel <= LogLevel.Information)
                {
                    loggerSettings.MinimumLogLevel = LogLevel.Trace;
                }

                var optionsMonitor =
                    _serviceProvider.GetService<IOptionsMonitor<LoggerFilterOptions>>() as
                        LanguageServerLoggerFilterOptions;

                if (optionsMonitor?.CurrentValue.MinLevel <= LogLevel.Information)
                {
                    optionsMonitor.CurrentValue.MinLevel = LogLevel.Trace;
                    optionsMonitor.Set(optionsMonitor.CurrentValue);
                }
            }

            _clientVersion = request.Capabilities?.GetClientVersion() ?? ClientVersion.Lsp2;
            _serializer.SetClientCapabilities(_clientVersion.Value, request.Capabilities);

            var supportedCapabilities = new List<ISupports>();
            if (_clientVersion == ClientVersion.Lsp3)
            {
                if (request.Capabilities.TextDocument != null)
                {
                    supportedCapabilities.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(request.Capabilities.TextDocument)
                    );
                }

                if (request.Capabilities.Workspace != null)
                {
                    supportedCapabilities.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(request.Capabilities.Workspace)
                    );
                }
            }

            _supportedCapabilities.Add(supportedCapabilities);

            ClientSettings.Capabilities ??= new ClientCapabilities();
            var textDocumentCapabilities =
                ClientSettings.Capabilities.TextDocument ??= new TextDocumentClientCapabilities();
            var workspaceCapabilities = ClientSettings.Capabilities.Workspace ??= new WorkspaceClientCapabilities();
            var windowCapabilities = ClientSettings.Capabilities.Window ??= new WindowClientCapabilities();
            _serverWorkDoneManager.Initialized(windowCapabilities);

            {
                RegisterHandlers(_collection);
            }

            await Task.WhenAll(_initializeDelegates.Select(c => c(this, request, token)));

            var ccp = new ClientCapabilityProvider(_collection, windowCapabilities.WorkDoneProgress);

            var serverCapabilities = new ServerCapabilities() {
                CodeActionProvider = ccp.GetStaticOptions(textDocumentCapabilities.CodeAction)
                    .Get<ICodeActionOptions, CodeActionOptions>(CodeActionOptions.Of),
                CodeLensProvider = ccp.GetStaticOptions(textDocumentCapabilities.CodeLens)
                    .Get<ICodeLensOptions, CodeLensOptions>(CodeLensOptions.Of),
                CompletionProvider = ccp.GetStaticOptions(textDocumentCapabilities.Completion)
                    .Get<ICompletionOptions, CompletionOptions>(CompletionOptions.Of),
                DefinitionProvider = ccp.GetStaticOptions(textDocumentCapabilities.Definition)
                    .Get<IDefinitionOptions, DefinitionOptions>(DefinitionOptions.Of),
                DocumentFormattingProvider = ccp.GetStaticOptions(textDocumentCapabilities.Formatting)
                    .Get<IDocumentFormattingOptions, DocumentFormattingOptions>(DocumentFormattingOptions.Of),
                DocumentHighlightProvider = ccp.GetStaticOptions(textDocumentCapabilities.DocumentHighlight)
                    .Get<IDocumentHighlightOptions, DocumentHighlightOptions>(DocumentHighlightOptions.Of),
                DocumentLinkProvider = ccp.GetStaticOptions(textDocumentCapabilities.DocumentLink)
                    .Get<IDocumentLinkOptions, DocumentLinkOptions>(DocumentLinkOptions.Of),
                DocumentOnTypeFormattingProvider = ccp.GetStaticOptions(textDocumentCapabilities.OnTypeFormatting)
                    .Get<IDocumentOnTypeFormattingOptions, DocumentOnTypeFormattingOptions>(
                        DocumentOnTypeFormattingOptions.Of),
                DocumentRangeFormattingProvider = ccp.GetStaticOptions(textDocumentCapabilities.RangeFormatting)
                    .Get<IDocumentRangeFormattingOptions, DocumentRangeFormattingOptions>(DocumentRangeFormattingOptions
                        .Of),
                DocumentSymbolProvider = ccp.GetStaticOptions(textDocumentCapabilities.DocumentSymbol)
                    .Get<IDocumentSymbolOptions, DocumentSymbolOptions>(DocumentSymbolOptions.Of),
                ExecuteCommandProvider = ccp.GetStaticOptions(workspaceCapabilities.ExecuteCommand)
                    .Reduce<IExecuteCommandOptions, ExecuteCommandOptions>(ExecuteCommandOptions.Of),
                TextDocumentSync = ccp.GetStaticOptions(textDocumentCapabilities.Synchronization)
                    .Reduce<ITextDocumentSyncOptions, TextDocumentSyncOptions>(TextDocumentSyncOptions.Of),
                HoverProvider = ccp.GetStaticOptions(textDocumentCapabilities.Hover)
                    .Get<IHoverOptions, HoverOptions>(HoverOptions.Of),
                ReferencesProvider = ccp.GetStaticOptions(textDocumentCapabilities.References)
                    .Get<IReferencesOptions, ReferencesOptions>(ReferencesOptions.Of),
                RenameProvider = ccp.GetStaticOptions(textDocumentCapabilities.Rename)
                    .Get<IRenameOptions, RenameOptions>(RenameOptions.Of),
                SignatureHelpProvider = ccp.GetStaticOptions(textDocumentCapabilities.SignatureHelp)
                    .Get<ISignatureHelpOptions, SignatureHelpOptions>(SignatureHelpOptions.Of),
                WorkspaceSymbolProvider = ccp.GetStaticOptions(workspaceCapabilities.Symbol)
                    .Get<IWorkspaceSymbolOptions, WorkspaceSymbolOptions>(WorkspaceSymbolOptions.Of),
                ImplementationProvider = ccp.GetStaticOptions(textDocumentCapabilities.Implementation)
                    .Get<IImplementationOptions, ImplementationOptions>(ImplementationOptions.Of),
                TypeDefinitionProvider = ccp.GetStaticOptions(textDocumentCapabilities.TypeDefinition)
                    .Get<ITypeDefinitionOptions, TypeDefinitionOptions>(TypeDefinitionOptions.Of),
                ColorProvider = ccp.GetStaticOptions(textDocumentCapabilities.ColorProvider)
                    .Get<IDocumentColorOptions, DocumentColorOptions>(DocumentColorOptions.Of),
                FoldingRangeProvider = ccp.GetStaticOptions(textDocumentCapabilities.FoldingRange)
                    .Get<IFoldingRangeOptions, FoldingRangeOptions>(FoldingRangeOptions.Of),
                SelectionRangeProvider = ccp.GetStaticOptions(textDocumentCapabilities.FoldingRange)
                    .Get<ISelectionRangeOptions, SelectionRangeOptions>(SelectionRangeOptions.Of),
                DeclarationProvider = ccp.GetStaticOptions(textDocumentCapabilities.Declaration)
                    .Get<IDeclarationOptions, DeclarationOptions>(DeclarationOptions.Of),
#pragma warning disable 618
                CallHierarchyProvider = ccp.GetStaticOptions(textDocumentCapabilities.CallHierarchy)
                    .Get<ICallHierarchyOptions, CallHierarchyOptions>(CallHierarchyOptions.Of),
                SemanticTokensProvider = ccp.GetStaticOptions(textDocumentCapabilities.SemanticTokens)
                    .Get<ISemanticTokensOptions, SemanticTokensOptions>(SemanticTokensOptions.Of),
#pragma warning restore 618
            };

            if (_collection.ContainsHandler(typeof(IDidChangeWorkspaceFoldersHandler)))
            {
                serverCapabilities.Workspace = new WorkspaceServerCapabilities() {
                    WorkspaceFolders = new WorkspaceFolderOptions() {
                        Supported = true,
                        ChangeNotifications = Guid.NewGuid().ToString()
                    }
                };
            }

            if (ccp.HasStaticHandler(textDocumentCapabilities.Synchronization))
            {
                var textDocumentSyncKind = TextDocumentSyncKind.None;
                if (_collection.ContainsHandler(typeof(IDidChangeTextDocumentHandler)))
                {
                    var kinds = _collection
                        .Select(x => x.Handler)
                        .OfType<IDidChangeTextDocumentHandler>()
                        .Select(x => x.GetRegistrationOptions()?.SyncKind ?? TextDocumentSyncKind.None)
                        .Where(x => x != TextDocumentSyncKind.None)
                        .ToArray();
                    if (kinds.Any())
                    {
                        textDocumentSyncKind = kinds.Min(z => z);
                    }
                }

                if (_clientVersion == ClientVersion.Lsp2)
                {
                    serverCapabilities.TextDocumentSync = textDocumentSyncKind;
                }
                else
                {
                    serverCapabilities.TextDocumentSync = new TextDocumentSyncOptions() {
                        Change = textDocumentSyncKind,
                        OpenClose = _collection.ContainsHandler(typeof(IDidOpenTextDocumentHandler)) ||
                                    _collection.ContainsHandler(typeof(IDidCloseTextDocumentHandler)),
                        Save = _collection.ContainsHandler(typeof(IDidSaveTextDocumentHandler))
                            ? new SaveOptions() {IncludeText = true /* TODO: Make configurable */}
                            : null,
                        WillSave = _collection.ContainsHandler(typeof(IWillSaveTextDocumentHandler)),
                        WillSaveWaitUntil = _collection.ContainsHandler(typeof(IWillSaveWaitUntilTextDocumentHandler))
                    };
                }
            }

            // TODO: Need a call back here
            // serverCapabilities.Experimental;

            _serverReceiver.Initialized();

            var result = ServerSettings = new InitializeResult() {
                Capabilities = serverCapabilities,
                ServerInfo = _serverInfo ?? new ServerInfo() {
                    Name = Assembly.GetEntryAssembly()?.GetName().Name,
                    Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                  ?.InformationalVersion ??
                              Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                }
            };

            await Task.WhenAll(_initializedDelegates.Select(c => c(this, request, result, token)));

            foreach (var item in _collection)
            {
                LspHandlerDescriptorHelpers.InitializeHandler(item, _supportedCapabilities, item.Handler);
            }

            // TODO:
            if (_clientVersion == ClientVersion.Lsp2)
            {
                _initializeComplete.OnNext(result);
                _initializeComplete.OnCompleted();
            }

            return result;
        }

        public async Task<MediatR.Unit> Handle(InitializedParams @params, CancellationToken token)
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                _initializeComplete.OnNext(ServerSettings);
                _initializeComplete.OnCompleted();
            }

            return MediatR.Unit.Value;
        }

        private async Task DynamicallyRegisterHandlers(Registration[] registrations)
        {
            if (registrations.Length == 0)
                return; // No dynamic registrations supported by client.

            await _initializeComplete;

            // Some clients (read: vscode) will throw errors on the first unhandled registration it encounters (and never registers the rest)
            // This is not really desirable as if we send a registration they don't support... that shouldn't break any unprocessed registrations
            // So in order to fix this we just send each registration along as separate requests all at once.
            await registrations
                .ToObservable()
                .Select(z => Observable.FromAsync(ct => Client.RegisterCapability(new RegistrationParams() {Registrations = new RegistrationContainer(z)}, ct))
                    .Catch<System.Reactive.Unit, Exception>(_ => {
                        this.LogWarning($"Unable to dynamically register capability '{z.Method}' perhaps it is not supported?");
                        return Observable.Empty<System.Reactive.Unit>();
                    })
                )
                .Merge()
                .ToTask();
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask();

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

        protected override IResponseRouter ResponseRouter => _responseRouter;
        protected override IHandlersManager HandlersManager => _collection;

        public IDisposable Register(Action<ILanguageServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeServerRegistry(_serviceProvider, manager, _textDocumentIdentifiers));
            return RegisterHandlers(manager.GetDisposable());
        }

        private IDisposable RegisterHandlers(IEnumerable<ILspHandlerDescriptor> collection)
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in collection)
            {
                if (descriptor.HasCapability && _supportedCapabilities.AllowsDynamicRegistration(descriptor.CapabilityType))
                {
                    if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                    {
                        wdpo.WorkDoneProgress = _serverWorkDoneManager.IsSupported;
                    }

                    registrations.Add(new Registration() {
                        Id = descriptor.Id.ToString(),
                        Method = descriptor.Method,
                        RegisterOptions = descriptor.RegistrationOptions
                    });
                }

                if (descriptor.OnServerStartedDelegate != null)
                {
                    // Fire and forget to initialize the handler
                    _initializeComplete
                        .Select(result =>
                            Observable.FromAsync((ct) => descriptor.OnServerStartedDelegate(this, result, ct)))
                        .Merge()
                        .Subscribe();
                }
            }

            // Fire and forget
            DynamicallyRegisterHandlers(registrations.ToArray()).ToObservable().Subscribe();

            return Disposable.Create(() => {
                Client.UnregisterCapability(new UnregistrationParams() {
                    Unregisterations = registrations.ToArray()
                }).ToObservable().Subscribe();
            });
        }

        private IDisposable RegisterHandlers(IDisposable handlerDisposable)
        {
            if (handlerDisposable is LspHandlerDescriptorDisposable lsp)
            {
                return new CompositeDisposable(lsp, RegisterHandlers(lsp.Descriptors));
            }

            if (!(handlerDisposable is CompositeDisposable cd)) return Disposable.Empty;
            cd.Add(RegisterHandlers(cd.OfType<LspHandlerDescriptorDisposable>().SelectMany(z => z.Descriptors)));
            return cd;
        }
    }

    class LangaugeServerRegistry : InterimLanguageProtocolRegistry<ILanguageServerRegistry>, ILanguageServerRegistry
    {
        public LangaugeServerRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            serviceProvider, handlersManager, textDocumentIdentifiers)
        {
        }
    }
}
