using System;
using System.Collections.Generic;
using System.Linq;
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
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;
using System.Reactive.Disposables;
using DryIoc;
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
        private readonly ILspServerReceiver _serverReceiver;
        private readonly ISerializer _serializer;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<OnLanguageServerInitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<OnLanguageServerInitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnLanguageServerStartedDelegate> _startedDelegates;
        private readonly IEnumerable<IOnLanguageServerStarted> _startedHandlers;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly ISupportedCapabilities _supportedCapabilities;
        private Task _initializingTask;
        private readonly LanguageProtocolSettingsBag _settingsBag;
        private bool _started;

        internal static IContainer CreateContainer(LanguageServerOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddLanguageServerInternals(options, outerServiceProvider);

        public static LanguageServer Create(LanguageServerOptions options) => Create(options, null);
        public static LanguageServer Create(Action<LanguageServerOptions> optionsAction) => Create(optionsAction, null);

        public static LanguageServer Create(Action<LanguageServerOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static LanguageServer Create(LanguageServerOptions options, IServiceProvider outerServiceProvider) =>
            CreateContainer(options, outerServiceProvider).Resolve<LanguageServer>();

        public static Task<LanguageServer> From(LanguageServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<LanguageServer> From(LanguageServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);

        public static Task<LanguageServer> From(LanguageServerOptions options, IServiceProvider outerServiceProvider) =>
            From(options, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, IServiceProvider outerServiceProvider) =>
            From(optionsAction, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<LanguageServer> From(LanguageServerOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static LanguageServer PreInit(Action<LanguageServerOptions> optionsAction)
        {
            return Create(optionsAction);
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
            return Create(options);
        }

        internal LanguageServer(
            Connection connection,
            IResponseRouter responseRouter,
            IOptions<LanguageServerOptions> options,
            ILanguageServerConfiguration configuration,
            ServerInfo serverInfo,
            ILspServerReceiver receiver,
            ISerializer serializer,
            IServiceProvider serviceProvider,
            ISupportedCapabilities supportedCapabilities,
            TextDocumentIdentifiers textDocumentIdentifiers,
            IEnumerable<OnLanguageServerInitializeDelegate> initializeDelegates,
            IEnumerable<OnLanguageServerInitializedDelegate> initializedDelegates,
            IEnumerable<OnLanguageServerStartedDelegate> startedDelegates,
            IEnumerable<IOnLanguageServerStarted> startedHandlers,
            IServerWorkDoneManager serverWorkDoneManager,
            ITextDocumentLanguageServer textDocumentLanguageServer,
            IClientLanguageServer clientLanguageServer,
            IGeneralLanguageServer generalLanguageServer,
            IWindowLanguageServer windowLanguageServer,
            IWorkspaceLanguageServer workspaceLanguageServer,
            LanguageProtocolSettingsBag languageProtocolSettingsBag,
            SharedHandlerCollection handlerCollection,
            IProgressManager progressManager,
            ILanguageServerWorkspaceFolderManager workspaceFolderManager) : base(handlerCollection, responseRouter)
        {
            Configuration = configuration;

            _connection = connection;
            _serverInfo = serverInfo;
            _serverReceiver = receiver;
            _serializer = serializer;
            _supportedCapabilities = supportedCapabilities;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _initializeDelegates = initializeDelegates;
            _initializedDelegates = initializedDelegates;
            _startedDelegates = startedDelegates;
            _startedHandlers = startedHandlers;
            WorkDoneManager = serverWorkDoneManager;
            _settingsBag = languageProtocolSettingsBag;
            Services = serviceProvider;
            _collection = handlerCollection;

            // We need to at least create Window here in case any handler does logging in their constructor
            TextDocument = textDocumentLanguageServer;
            Client = clientLanguageServer;
            General = generalLanguageServer;
            Window = windowLanguageServer;
            Workspace = workspaceLanguageServer;
            ProgressManager = progressManager;
            WorkspaceFolderManager = workspaceFolderManager;

            _disposable.Add(_collection.Add(this));
        }


        public ITextDocumentLanguageServer TextDocument { get; }
        public IClientLanguageServer Client { get; }
        public IGeneralLanguageServer General { get; }
        public IWindowLanguageServer Window { get; }
        public IWorkspaceLanguageServer Workspace { get; }

        public InitializeParams ClientSettings
        {
            get => _settingsBag.ClientSettings;
            private set => _settingsBag.ClientSettings = value;
        }

        public InitializeResult ServerSettings
        {
            get => _settingsBag.ServerSettings;
            private set => _settingsBag.ServerSettings = value;
        }

        public IServiceProvider Services { get; }
        public IProgressManager ProgressManager { get; }
        public IServerWorkDoneManager WorkDoneManager { get; }
        public ILanguageServerConfiguration Configuration { get; }
        public ILanguageServerWorkspaceFolderManager WorkspaceFolderManager { get; }

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
                async Task InitializeAll()
                {
                    var result = await _initializeComplete.ToTask(token);
                    var tasks =
                        _startedDelegates.Select(handler => handler(this, result, token))
                        .Concat(_startedHandlers
                            .Union(_collection.Select(z => z.Handler).OfType<IOnLanguageServerStarted>())
                            .Select(handler => handler.OnStarted(this, result, token))
                        )
                        .ToArray();
                    if (tasks.Length > 0)
                    {
                        await Task.WhenAll(tasks);
                    }
                }
                _initializingTask = InitializeAll();
                await _initializingTask;

                _started = true;
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

            if (request.Trace == InitializeTrace.Verbose)
            {
                var loggerSettings = Services.GetService<LanguageServerLoggerSettings>();

                if (loggerSettings?.MinimumLogLevel <= LogLevel.Information)
                {
                    loggerSettings.MinimumLogLevel = LogLevel.Trace;
                }

                var optionsMonitor =
                    Services.GetService<IOptionsMonitor<LoggerFilterOptions>>() as
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
            WorkDoneManager.Initialized(windowCapabilities);

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
                ServerInfo = _serverInfo
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

            var @params = new RegistrationParams() {Registrations = registrations};

            await _initializeComplete;

            await Client.RegisterCapability(@params);
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask();

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();

        public IDisposable Register(Action<ILanguageServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeServerRegistry(Services, manager, _textDocumentIdentifiers));

            var result = manager.GetDisposable();
            if (_started)
            {
                foreach (var item in result.OfType<LspHandlerDescriptorDisposable>())
                foreach (var descriptor in item.Descriptors)
                {
                    if (descriptor.Handler is IOnLanguageServerStarted serverStarted)
                    {
                        Observable.FromAsync((ct) => serverStarted.OnStarted(this, ServerSettings, ct)).Subscribe();
                    }
                }
            }
            return RegisterHandlers(result);
        }

        private IDisposable RegisterHandlers(IEnumerable<ILspHandlerDescriptor> collection)
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in collection)
            {
                if (descriptor is LspHandlerDescriptor lspHandlerDescriptor &&
                    lspHandlerDescriptor.TypeDescriptor?.HandlerType != null &&
                    typeof(IDoesNotParticipateInRegistration).IsAssignableFrom(lspHandlerDescriptor.TypeDescriptor.HandlerType))
                {
                    continue;
                }

                if (descriptor.HasCapability && _supportedCapabilities.AllowsDynamicRegistration(descriptor.CapabilityType))
                {
                    if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                    {
                        wdpo.WorkDoneProgress = WorkDoneManager.IsSupported;
                    }

                    registrations.Add(new Registration() {
                        Id = descriptor.Id.ToString(),
                        Method = descriptor.Method,
                        RegisterOptions = descriptor.RegistrationOptions
                    });
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

        object IServiceProvider.GetService(Type serviceType) => Services.GetService(serviceType);
    }
}
