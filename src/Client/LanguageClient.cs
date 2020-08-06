using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Shared;
using OmniSharp.Extensions.JsonRpc.DryIoc;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class LanguageClientServiceCollectionExtensions
    {
        internal static IContainer AddLanguageClientInternals(this IContainer container, LanguageClientOptions options, IServiceProvider outerServiceProvider)
        {
            if (options.Receiver == null)
            {
                throw new ArgumentException("Receiver is missing!", nameof(options));
            }

            container  = container.AddLanguageProtocolInternals(options);

            container.RegisterMany<WorkspaceFoldersManager>(serviceTypeCondition: type => options.WorkspaceFolders || type != typeof(IJsonRpcHandler));

            container.RegisterInstance(options.ClientCapabilities);
            container.RegisterInstanceMany(options.Receiver);
            if (options.OnUnhandledException == null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => _.GetRequiredService<LanguageClient>().Shutdown()));
            }

            container.RegisterMany<TextDocumentLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(TextDocumentLanguageClient)));
            container.RegisterMany<ClientLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(ClientLanguageClient)));
            container.RegisterMany<GeneralLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(GeneralLanguageClient)));
            container.RegisterMany<WindowLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(WindowLanguageClient)));
            container.RegisterMany<WorkspaceLanguageClient>(serviceTypeCondition: type => type.Name.Contains(nameof(WorkspaceLanguageClient)));
            container.RegisterInstance<IOptionsFactory<LanguageClientOptions>>(new ValueOptionsFactory<LanguageClientOptions>(options));

            container.RegisterMany<LanguageClient>(serviceTypeCondition: type => type == typeof(ILanguageClient) || type == typeof(LanguageClient)/*, reuse: Reuse.Singleton*/);
            container.RegisterInitializer<LanguageClient>((client, context) => {
                var manager = context.Resolve<IHandlersManager>();
                var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                descriptions.Populate(context, manager);

                var handlers = context.ResolveMany<IJsonRpcHandler>();
                foreach (var handler in handlers)
                {
                    manager.Add(handler, new JsonRpcHandlerOptions());
                }
            });

            container.RegisterInstance(options.ClientInfo ?? new ClientInfo() {
                Name = Assembly.GetEntryAssembly()?.GetName().Name,
                Version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                              ?.InformationalVersion ??
                          Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
            });

            container.RegisterMany<ClientWorkDoneManager>();
            container.RegisterMany<RegistrationManager>(serviceTypeCondition: type => options.DynamicRegistration || type != typeof(IJsonRpcHandler));

            return container;
        }

        public static IServiceCollection AddLanguageClient(this IServiceCollection services, Action<LanguageClientOptions> configureOptions = null)
        {
            return AddLanguageClient(services, Options.DefaultName, configureOptions);
        }

        public static IServiceCollection AddLanguageClient(this IServiceCollection services, string name, Action<LanguageClientOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(LanguageClient) || d.ServiceType == typeof(ILanguageClient)))
            {
                services.RemoveAll<LanguageClient>();
                services.RemoveAll<ILanguageClient>();
                services.AddSingleton<ILanguageClient>(_ =>
                    throw new NotSupportedException("LanguageClient has been registered multiple times, you must use LanguageClient instead"));
                services.AddSingleton<LanguageClient>(_ =>
                    throw new NotSupportedException("LanguageClient has been registered multiple times, you must use LanguageClient instead"));
            }

            services
                .AddOptions()
                .AddLogging();
            services.TryAddSingleton<LanguageClientResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<LanguageClientResolver>().Get(name));
            services.TryAddSingleton<ILanguageClient>(_ => _.GetRequiredService<LanguageClientResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }

    public class LanguageClientResolver : IDisposable
    {
        private readonly IOptionsMonitor<LanguageClientOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, LanguageClient> _clients = new ConcurrentDictionary<string, LanguageClient>();

        public LanguageClientResolver(IOptionsMonitor<LanguageClientOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public LanguageClient Get(string name)
        {
            if (_clients.TryGetValue(name, out var client)) return client;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = LanguageClient.CreateContainer(options, _outerServiceProvider);
            client = container.Resolve<LanguageClient>();
            _clients.TryAdd(name, client);

            return client;
        }

        public void Dispose()
        {
            foreach (var item in _clients.Values) item.Dispose();
        }
    }

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

        // private readonly ILanguageClientConfiguration _configuration;
        private readonly IEnumerable<ICapability> _capabilities;
        private readonly object _initializationOptions;
        private readonly DocumentUri _rootUri;
        private readonly InitializeTrace _trace;
        private readonly ClientCapabilities _clientCapabilities;
        private readonly LanguageProtocolSettingsBag _settingsBag;

        internal static IContainer CreateContainer(LanguageClientOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddLanguageClientInternals(options, outerServiceProvider)
                .Populate(options.Services);

        public static LanguageClient Create(LanguageClientOptions options) => Create(options, null);
        public static LanguageClient Create(Action<LanguageClientOptions> optionsAction) => Create(optionsAction, null);

        public static LanguageClient Create(Action<LanguageClientOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static LanguageClient Create(LanguageClientOptions options, IServiceProvider outerServiceProvider) => CreateContainer(options, outerServiceProvider).Resolve<LanguageClient>();
        public static Task<LanguageClient> From(LanguageClientOptions options) => From(options, null, CancellationToken.None);
        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<LanguageClient> From(LanguageClientOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);

        public static Task<LanguageClient> From(LanguageClientOptions options, IServiceProvider outerServiceProvider) =>
            From(options, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction, IServiceProvider outerServiceProvider) =>
            From(optionsAction, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageClient> From(Action<LanguageClientOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<LanguageClient> From(LanguageClientOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
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
            ILspClientReceiver lspClientReceiver,
            TextDocumentIdentifiers textDocumentIdentifiers,
            IServiceProvider serviceProvider,
            IEnumerable<OnClientStartedDelegate> startedDelegates,
            ITextDocumentLanguageClient textDocumentLanguageClient,
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
            IWorkspaceFoldersManager workspaceFoldersManager
        ) : base(options.Value)
        {
            _connection = connection;
            _capabilities = capabilities;
            _clientCapabilities = clientCapabilities;
            _clientInfo = clientInfo;
            _receiver = lspClientReceiver;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _startedDelegates = startedDelegates;
            _rootUri = options.Value.RootUri;
            _trace = options.Value.Trace;
            _initializationOptions = options.Value.InitializationOptions;
            _settingsBag = languageProtocolSettingsBag;
            _collection = handlerCollection;
            Services = serviceProvider;

            _responseRouter = responseRouter;
            ProgressManager = progressManager;
            WorkDoneManager = clientWorkDoneManager;
            RegistrationManager = registrationManager;
            WorkspaceFoldersManager = workspaceFoldersManager;

            // We need to at least create Window here in case any handler does loggin in their constructor
            TextDocument = textDocumentLanguageClient;
            Client = clientLanguageClient;
            General = generalLanguageClient;
            Window = windowLanguageClient;
            Workspace = workspaceLanguageClient;

            WorkspaceFoldersManager.Add(options.Value.Folders.ToArray());

            var serviceIdentifiers = Services.GetServices<ITextDocumentIdentifier>().ToArray();
            _disposable.Add(_textDocumentIdentifiers.Add(serviceIdentifiers));
        }

        public ITextDocumentLanguageClient TextDocument { get; }
        public IClientLanguageClient Client { get; }
        public IGeneralLanguageClient General { get; }
        public IWindowLanguageClient Window { get; }
        public IWorkspaceLanguageClient Workspace { get; }
        public IProgressManager ProgressManager { get; }

        public IClientWorkDoneManager WorkDoneManager { get; }

        public IRegistrationManager RegistrationManager { get; }

        public IWorkspaceFoldersManager WorkspaceFoldersManager { get; }


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
            var @params = new InitializeParams {
                Trace = _trace,
                Capabilities = _clientCapabilities,
                ClientInfo = _clientInfo,
                RootUri = _rootUri,
                RootPath = _rootUri?.GetFileSystemPath(),
                WorkspaceFolders = new Container<WorkspaceFolder>(WorkspaceFoldersManager.CurrentWorkspaceFolders),
                InitializationOptions = _initializationOptions
            };
            RegisterCapabilities(@params.Capabilities);
            WorkDoneManager.Initialize(@params.Capabilities.Window);

            ClientSettings = @params;

            _connection.Open();
            var serverParams = await this.RequestLanguageProtocolInitialize(ClientSettings, token);
            _receiver.Initialized();

            await _startedDelegates.Select(@delegate =>
                    Observable.FromAsync(() => @delegate(this, serverParams, token))
                )
                .ToObservable()
                .Merge()
                .LastOrDefaultAsync()
                .ToTask(token);

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
        protected override IResponseRouter ResponseRouter => _responseRouter;
        protected override IHandlersManager HandlersManager => _collection;

        public IDisposable Register(Action<ILanguageClientRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeClientRegistry(Services, manager, _textDocumentIdentifiers));
            return manager.GetDisposable();
        }
    }
}
