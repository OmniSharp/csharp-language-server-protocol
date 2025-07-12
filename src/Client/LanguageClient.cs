using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using DryIoc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Shared;

// ReSharper disable SuspiciousTypeConversion.Global

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [BuiltIn]
    public class LanguageClient : JsonRpcServerBase, ILanguageClient
    {
        private readonly Connection _connection;
        private readonly ClientInfo _clientInfo;
        private readonly IReceiver _receiver;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        private readonly SharedHandlerCollection _collection;

        // private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        // private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnLanguageClientStartedDelegate> _startedDelegates;
        private readonly IEnumerable<IOnLanguageClientStarted> _startedHandlers;
        private readonly IEnumerable<OnLanguageClientInitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<IOnLanguageClientInitialize> _initializeHandlers;
        private readonly IEnumerable<OnLanguageClientInitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<IOnLanguageClientInitialized> _initializedHandlers;
        private readonly LspSerializer _serializer;
        private readonly InstanceHasStarted _instanceHasStarted;
        private readonly IScheduler _scheduler;
        private readonly IResponseRouter _responseRouter;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        // private readonly ILanguageClientConfiguration _configuration;
        private readonly IEnumerable<ICapability> _capabilities;
        private readonly object? _initializationOptions;
        private readonly DocumentUri? _rootUri;
        private readonly InitializeTrace _trace;
        private readonly ClientCapabilities _clientCapabilities;
        private readonly LanguageProtocolSettingsBag _settingsBag;
        private readonly int? _concurrency;
        private readonly IResolverContext _resolverContext;

        internal static IContainer CreateContainer(LanguageClientOptions options, IServiceProvider? outerServiceProvider)
        {
            return JsonRpcServerContainer.Create(outerServiceProvider)
                                         .AddLanguageClientInternals(options, outerServiceProvider);
        }

        public static LanguageClient Create(LanguageClientOptions options)
        {
            return Create(options, null);
        }

        public static LanguageClient Create(Action<LanguageClientOptions> optionsAction)
        {
            return Create(optionsAction, null);
        }

        public static LanguageClient Create(Action<LanguageClientOptions> optionsAction, IServiceProvider? outerServiceProvider)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static LanguageClient Create(LanguageClientOptions options, IServiceProvider? outerServiceProvider)
        {
            return CreateContainer(options, outerServiceProvider).Resolve<LanguageClient>();
        }

        public static Task<LanguageClient> From(LanguageClientOptions options)
        {
            return From(options, null, CancellationToken.None);
        }

        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction)
        {
            return From(optionsAction, null, CancellationToken.None);
        }

        public static Task<LanguageClient> From(LanguageClientOptions options, CancellationToken cancellationToken)
        {
            return From(options, null, cancellationToken);
        }

        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction, CancellationToken cancellationToken)
        {
            return From(optionsAction, null, cancellationToken);
        }

        public static Task<LanguageClient> From(LanguageClientOptions options, IServiceProvider? outerServiceProvider)
        {
            return From(options, outerServiceProvider, CancellationToken.None);
        }

        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction, IServiceProvider? outerServiceProvider)
        {
            return From(optionsAction, outerServiceProvider, CancellationToken.None);
        }

        public static Task<LanguageClient> From(
            Action<LanguageClientOptions> optionsAction, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken
        )
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<LanguageClient> From(
            LanguageClientOptions options, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken
        )
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken).ConfigureAwait(false);
            return server;
        }

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static LanguageClient PreInit(Action<LanguageClientOptions> optionsAction)
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
        public static LanguageClient PreInit(LanguageClientOptions options)
        {
            return Create(options);
        }

        internal LanguageClient(
            Connection connection,
            IOptions<LanguageClientOptions> options,
            IEnumerable<ICapability> capabilities,
            ClientInfo clientInfo,
            ClientCapabilities clientCapabilities,
            IReceiver lspClientReceiver,
            TextDocumentIdentifiers textDocumentIdentifiers,
            IResolverContext resolverContext,
            IEnumerable<OnLanguageClientStartedDelegate> startedDelegates,
            IEnumerable<IOnLanguageClientStarted> startedHandlers,
            ITextDocumentLanguageClient textDocumentLanguageClient,
            INotebookDocumentLanguageClient notebookDocumentLanguageClient,
            IClientLanguageClient clientLanguageClient,
            IGeneralLanguageClient generalLanguageClient,
            IWindowLanguageClient windowLanguageClient,
            IWorkspaceLanguageClient workspaceLanguageClient,
            LanguageProtocolSettingsBag languageProtocolSettingsBag,
            SharedHandlerCollection handlerCollection,
            IResponseRouter responseRouter,
            IProgressManager progressManager,
            IClientWorkDoneManager clientWorkDoneManager,
            IRegistrationManager registrationManager,
            ILanguageClientWorkspaceFoldersManager languageClientWorkspaceFoldersManager,
            IEnumerable<OnLanguageClientInitializeDelegate> initializeDelegates,
            IEnumerable<IOnLanguageClientInitialize> initializeHandlers,
            IEnumerable<OnLanguageClientInitializedDelegate> initializedDelegates,
            IEnumerable<IOnLanguageClientInitialized> initializedHandlers,
            LspSerializer serializer,
            InstanceHasStarted instanceHasStarted,
            IScheduler scheduler
        ) : base(handlerCollection, responseRouter)
        {
            _connection = connection;
            _capabilities = capabilities;
            _clientCapabilities = serializer.DeserializeObject<ClientCapabilities>(serializer.SerializeObject(clientCapabilities));
            _clientInfo = clientInfo;
            _receiver = lspClientReceiver;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _startedDelegates = startedDelegates;
            _startedHandlers = startedHandlers;
            _rootUri = options.Value.RootUri;
            _trace = options.Value.Trace;
            _initializationOptions = options.Value.InitializationOptions;
            _settingsBag = languageProtocolSettingsBag;
            _collection = handlerCollection;
            Services = _resolverContext = resolverContext;

            _responseRouter = responseRouter;
            ProgressManager = progressManager;
            WorkDoneManager = clientWorkDoneManager;
            RegistrationManager = registrationManager;
            WorkspaceFoldersManager = languageClientWorkspaceFoldersManager;
            _initializeDelegates = initializeDelegates;
            _initializeHandlers = initializeHandlers;
            _initializedDelegates = initializedDelegates;
            _initializedHandlers = initializedHandlers;
            _serializer = serializer;
            _instanceHasStarted = instanceHasStarted;
            _scheduler = scheduler;
            _concurrency = options.Value.Concurrency;

            // We need to at least create Window here in case any handler does loggin in their constructor
            TextDocument = textDocumentLanguageClient;
            NotebookDocument = notebookDocumentLanguageClient;
            Client = clientLanguageClient;
            General = generalLanguageClient;
            Window = windowLanguageClient;
            Workspace = workspaceLanguageClient;
        }

        public ITextDocumentLanguageClient TextDocument { get; }
        public INotebookDocumentLanguageClient NotebookDocument { get; }
        public IClientLanguageClient Client { get; }
        public IGeneralLanguageClient General { get; }
        public IWindowLanguageClient Window { get; }
        public IWorkspaceLanguageClient Workspace { get; }
        public IProgressManager ProgressManager { get; }
        public IClientWorkDoneManager WorkDoneManager { get; }
        public IRegistrationManager RegistrationManager { get; }
        public ILanguageClientWorkspaceFoldersManager WorkspaceFoldersManager { get; }

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

        public async Task Initialize(CancellationToken token)
        {
            var @params = new InitializeParams
            {
                Trace = _trace,
                ClientInfo = _clientInfo,
                Capabilities = _clientCapabilities,
                RootUri = _rootUri!,
                RootPath = _rootUri?.GetFileSystemPath() ?? string.Empty,
                WorkspaceFolders = new Container<WorkspaceFolder>(WorkspaceFoldersManager.CurrentWorkspaceFolders),
                InitializationOptions = _initializationOptions!
            };

            var capabilitiesObject = new JObject();
            foreach (var capability in _capabilities)
            {
                var keys = capability.GetType().GetCustomAttribute<CapabilityKeyAttribute>()?.Keys.Select(key => char.ToLower(key[0]) + key.Substring(1))
                                     .ToArray();
                if (keys != null)
                {
                    var value = capabilitiesObject;
                    foreach (var key in keys.Take(keys.Length - 1))
                    {
                        if (value.TryGetValue(key, out var t) && t is JObject to)
                        {
                            value = to;
                        }
                        else
                        {
                            value[key] = value = new JObject();
                        }
                    }

                    var lastKey = keys[keys.Length - 1];
                    value[lastKey] = JToken.FromObject(capability, _serializer.JsonSerializer);
                }
            }

            using (var reader = capabilitiesObject.CreateReader())
            {
                _serializer.JsonSerializer.Populate(reader, _clientCapabilities);
            }

            _collection.Initialize();
            RegisterCapabilities(_clientCapabilities);

            await LanguageProtocolEventingHelper.Run(
                _initializeDelegates,
                (handler, ct) => handler(this, @params, ct),
                _initializeHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageClientInitialize>()),
                (handler, ct) => handler.OnInitialize(this, @params, ct),
                _concurrency,
                _scheduler,
                token
            ).ConfigureAwait(false);

            WorkDoneManager.Initialize(@params.Capabilities.Window);

            ClientSettings = @params;

            _connection.Open();
            var serverParams = await SendRequest(ClientSettings, token).ConfigureAwait(false);

            ServerSettings = serverParams;
            _serializer.SetServerCapabilities(serverParams.Capabilities);

            await LanguageProtocolEventingHelper.Run(
                _initializedDelegates,
                (handler, ct) => handler(this, @params, serverParams, ct),
                _initializedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageClientInitialized>()),
                (handler, ct) => handler.OnInitialized(this, @params, serverParams, ct),
                _concurrency,
                _scheduler,
                token
            ).ConfigureAwait(false);

            _receiver.Initialized();
            // post init

            if (_collection.ContainsHandler(typeof(IRegisterCapabilityHandler)))
                RegistrationManager.RegisterCapabilities(serverParams.Capabilities);

            // TODO: pull supported fields and add any static registrations to the registration manager
            this.SendLanguageProtocolInitialized(new InitializedParams());

            await LanguageProtocolEventingHelper.Run(
                _startedDelegates,
                (handler, ct) => handler(this, ct),
                _startedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageClientStarted>()),
                (handler, ct) => handler.OnStarted(this, ct),
                _concurrency,
                _scheduler,
                token
            ).ConfigureAwait(false);

            _instanceHasStarted.Started = true;
        }

        private void RegisterCapabilities(ClientCapabilities capabilities)
        {
            capabilities.General ??= _serializer.DeserializeObject<GeneralClientCapabilities>("{}");
            capabilities.Window ??= _serializer.DeserializeObject<WindowClientCapabilities>("{}");
            capabilities.Window.WorkDoneProgress = _collection.ContainsHandler(typeof(IProgressHandler));

            capabilities.Workspace ??= _serializer.DeserializeObject<WorkspaceClientCapabilities>("{}");
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

            capabilities.TextDocument ??= _serializer.DeserializeObject<TextDocumentClientCapabilities>("{}");
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
                await this.RequestShutdown().ConfigureAwait(false);
                this.SendExit();
            }

            await _connection.StopAsync().ConfigureAwait(false);
            _connection.Dispose();
        }

        private Supports<T> UseOrTryAndFindCapability<T>(Supports<T> supports) where T : class?
        {
            var value = supports.IsSupported
                ? supports.Value
                : _capabilities.OfType<T>().FirstOrDefault()!;
            if (value is IDynamicCapability dynamicCapability)
            {
                dynamicCapability.DynamicRegistration = _collection.ContainsHandler(typeof(IRegisterCapabilityHandler));
            }

            return Supports.OfValue(value);
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        bool IResponseRouter.TryGetRequest(long id, [NotNullWhen(true)] out string? method, [NotNullWhen(true)] out TaskCompletionSource<JToken>? pendingTask)
        {
            return _responseRouter.TryGetRequest(id, out method, out pendingTask);
        }

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask(_scheduler);

        public void Dispose()
        {
            _connection.Dispose();
            _disposable.Dispose();
        }

        public IDisposable Register(Action<ILanguageClientRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeClientRegistry(_resolverContext, manager, _textDocumentIdentifiers));
            var result = manager.GetDisposable();
            if (_instanceHasStarted.Started)
            {
                LanguageClientHelpers.InitHandlers(this, result);
            }

            return result;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return Services.GetService(serviceType);
        }
    }
}
