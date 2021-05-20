using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Logging;
using OmniSharp.Extensions.LanguageServer.Shared;

// ReSharper disable SuspiciousTypeConversion.Global

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [BuiltIn]
    public partial class LanguageServer : JsonRpcServerBase, ILanguageServer, ILanguageProtocolInitializeHandler, ILanguageProtocolInitializedHandler, IAwaitableTermination
    {
        private readonly Connection _connection;
        private ClientVersion? _clientVersion;
        private readonly ServerInfo _serverInfo;
        private readonly IReceiver _serverReceiver;
        private readonly LspSerializer _serializer;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        private readonly SharedHandlerCollection _collection;
        private readonly IEnumerable<OnLanguageServerInitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<IOnLanguageServerInitialize> _initializeHandlers;
        private readonly IEnumerable<OnLanguageServerInitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<IOnLanguageServerInitialized> _initializedHandlers;
        private readonly IEnumerable<IRegistrationOptionsConverter> _registrationOptionsConverters;
        private readonly InstanceHasStarted _instanceHasStarted;
        private readonly LanguageServerLoggingManager _languageServerLoggingManager;
        private readonly IScheduler _scheduler;
        private readonly IEnumerable<OnLanguageServerStartedDelegate> _startedDelegates;
        private readonly IEnumerable<IOnLanguageServerStarted> _startedHandlers;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly ISupportedCapabilities _supportedCapabilities;
        private Task? _initializingTask;
        private readonly LanguageProtocolSettingsBag _settingsBag;
        private readonly int? _concurrency;
        private readonly ILookup<string, Type> _capabilityTypes;
        private readonly IResolverContext _resolverContext;

        internal static IContainer CreateContainer(LanguageServerOptions options, IServiceProvider? outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                                  .AddLanguageServerInternals(options, outerServiceProvider);

        public static LanguageServer Create(LanguageServerOptions options) => Create(options, null);
        public static LanguageServer Create(Action<LanguageServerOptions> optionsAction) => Create(optionsAction, null);

        public static LanguageServer Create(Action<LanguageServerOptions> optionsAction, IServiceProvider? outerServiceProvider)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static LanguageServer Create(LanguageServerOptions options, IServiceProvider? outerServiceProvider) =>
            CreateContainer(options, outerServiceProvider).Resolve<LanguageServer>();

        public static Task<LanguageServer> From(LanguageServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<LanguageServer> From(LanguageServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);

        public static Task<LanguageServer> From(LanguageServerOptions options, IServiceProvider? outerServiceProvider) =>
            From(options, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, IServiceProvider? outerServiceProvider) =>
            From(optionsAction, outerServiceProvider, CancellationToken.None);

        public static Task<LanguageServer> From(Action<LanguageServerOptions> optionsAction, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<LanguageServer> From(LanguageServerOptions options, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken)
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
        public static LanguageServer PreInit(Action<LanguageServerOptions> optionsAction) => Create(optionsAction);

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static LanguageServer PreInit(LanguageServerOptions options) => Create(options);

        internal LanguageServer(
            Connection connection,
            IResponseRouter responseRouter,
            IOptions<LanguageServerOptions> options,
            ILanguageServerConfiguration configuration,
            ServerInfo serverInfo,
            IReceiver receiver,
            LspSerializer serializer,
            IResolverContext resolverContext,
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
            ILanguageServerWorkspaceFolderManager workspaceFolderManager, IEnumerable<IOnLanguageServerInitialize> initializeHandlers,
            IEnumerable<IOnLanguageServerInitialized> initializedHandlers,
            IEnumerable<IRegistrationOptionsConverter> registrationOptionsConverters,
            InstanceHasStarted instanceHasStarted,
            LanguageServerLoggingManager languageServerLoggingManager,
            IScheduler scheduler
        ) : base(handlerCollection, responseRouter)
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
            Services = _resolverContext = resolverContext;
            _collection = handlerCollection;

            // We need to at least create Window here in case any handler does logging in their constructor
            TextDocument = textDocumentLanguageServer;
            Client = clientLanguageServer;
            General = generalLanguageServer;
            Window = windowLanguageServer;
            Workspace = workspaceLanguageServer;
            ProgressManager = progressManager;
            WorkspaceFolderManager = workspaceFolderManager;
            _initializeHandlers = initializeHandlers;
            _initializedHandlers = initializedHandlers;
            _registrationOptionsConverters = registrationOptionsConverters;
            _instanceHasStarted = instanceHasStarted;
            _languageServerLoggingManager = languageServerLoggingManager;
            _scheduler = scheduler;
            _concurrency = options.Value.Concurrency;

            _capabilityTypes = options.Value.UseAssemblyAttributeScanning
                ? options.Value.Assemblies
                         .SelectMany(z => z.GetCustomAttributes<AssemblyCapabilityKeyAttribute>())
                         .ToLookup(z => z.CapabilityKey, z => z.CapabilityType)
                : options.Value.Assemblies
                         .SelectMany(z => z.ExportedTypes)
                         .Where(z => z.IsClass && !z.IsAbstract)
                         .Where(z => typeof(ICapability).IsAssignableFrom(z))
                         .Where(z => z.GetCustomAttributes<CapabilityKeyAttribute>().Any())
                         .ToLookup(z => string.Join(".", z.GetCustomAttribute<CapabilityKeyAttribute>().Keys));

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
                    await _initializingTask.ConfigureAwait(false);
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
                _initializingTask = _initializeComplete.ToTask(token, _scheduler);
                await _initializingTask.ConfigureAwait(false);
                await LanguageProtocolEventingHelper.Run(
                    _startedDelegates,
                    (handler, ct) => handler(this, ct),
                    _startedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageServerStarted>()),
                    (handler, ct) => handler.OnStarted(this, ct),
                    _concurrency,
                    _scheduler,
                    token
                ).ConfigureAwait(false);

                _instanceHasStarted.Started = true;
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

        async Task<InitializeResult> IRequestHandler<InternalInitializeParams, InitializeResult>.Handle(
            InternalInitializeParams request, CancellationToken token
        )
        {
            ConfigureServerLogging(request);

            ReadClientCapabilities(request, out var clientCapabilities, out var textDocumentCapabilities, out _, out var windowCapabilities, out _);

            await LanguageProtocolEventingHelper.Run(
                _initializeDelegates,
                (handler, ct) => handler(this, ClientSettings, ct),
                _initializeHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageServerInitialize>()),
                (handler, ct) => handler.OnInitialize(this, ClientSettings, ct),
                _concurrency,
                _scheduler,
                token
            ).ConfigureAwait(false);

            _disposable.Add(
                LanguageServerHelpers.RegisterHandlers(
                    _initializeComplete.Select(_ => System.Reactive.Unit.Default),
                    Client,
                    WorkDoneManager,
                    _supportedCapabilities,
                    _collection
                )
            );

            var result = ReadServerCapabilities(clientCapabilities, windowCapabilities, textDocumentCapabilities);

            await LanguageProtocolEventingHelper.Run(
                _initializedDelegates,
                (handler, ct) => handler(this, ClientSettings, result, ct),
                _initializedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnLanguageServerInitialized>()),
                (handler, ct) => handler.OnInitialized(this, ClientSettings, result, ct),
                _concurrency,
                _scheduler,
                token
            ).ConfigureAwait(false);


            // Allow the server receiver to start processing incoming notifications and requests. It
            // is necessary to do this now, and not in the Initialized handler, because otherwise
            // clients can enter a race with the receiver and have their notifications and requests
            // erroneously dropped.
            _serverReceiver.Initialized();

            // TODO:
            if (_clientVersion == ClientVersion.Lsp2)
            {
                _initializeComplete.OnNext(result);
                _initializeComplete.OnCompleted();
            }

            return result;
        }

        public Task<Unit> Handle(InitializedParams @params, CancellationToken token)
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                _initializeComplete.OnNext(ServerSettings);
                _initializeComplete.OnCompleted();
            }

            return Unit.Task;
        }

        private void ConfigureServerLogging(InternalInitializeParams internalInitializeParams)
        {
            _languageServerLoggingManager.SetTrace(internalInitializeParams.Trace);
        }

        private void ReadClientCapabilities(
            InternalInitializeParams request,
            out ClientCapabilities clientCapabilities,
            out TextDocumentClientCapabilities textDocumentCapabilities,
            out WorkspaceClientCapabilities workspaceCapabilities,
            out WindowClientCapabilities windowCapabilities,
            out GeneralClientCapabilities generalCapabilities
        )
        {
            clientCapabilities = request.Capabilities.ToObject<ClientCapabilities>(_serializer.JsonSerializer);
            _supportedCapabilities.Initialize(clientCapabilities);
            foreach (var group in _capabilityTypes)
            {
                foreach (var capabilityType in @group)
                {
                    if (request.Capabilities.SelectToken(@group.Key) is JObject capabilityData)
                    {
                        var capability = capabilityData.ToObject(capabilityType) as ICapability;
                        _supportedCapabilities.Add(capability!);
                    }
                }
            }

            _clientVersion = clientCapabilities.GetClientVersion();
            _serializer.SetClientCapabilities(clientCapabilities);

            var supportedCapabilities = new List<ISupports>();
            if (_clientVersion == ClientVersion.Lsp3)
            {
                if (clientCapabilities.TextDocument != null)
                {
                    supportedCapabilities.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(clientCapabilities.TextDocument)
                    );
                }

                if (clientCapabilities.Workspace != null)
                {
                    supportedCapabilities.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(clientCapabilities.Workspace)
                    );
                }

                if (clientCapabilities.Window != null)
                {
                    supportedCapabilities.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(clientCapabilities.Window)
                    );
                }
            }

            _supportedCapabilities.Add(supportedCapabilities);

            ClientSettings = new InitializeParams(request, clientCapabilities) { Capabilities = clientCapabilities };
            textDocumentCapabilities = ClientSettings.Capabilities.TextDocument ??= _serializer.DeserializeObject<TextDocumentClientCapabilities>("{}");
            workspaceCapabilities = ClientSettings.Capabilities.Workspace ??= _serializer.DeserializeObject<WorkspaceClientCapabilities>("{}");
            windowCapabilities = ClientSettings.Capabilities.Window ??= _serializer.DeserializeObject<WindowClientCapabilities>("{}");
            generalCapabilities = ClientSettings.Capabilities.General ??= _serializer.DeserializeObject<GeneralClientCapabilities>("{}");
            WorkDoneManager.Initialized(windowCapabilities);
            _collection.Initialize();
        }

        private InitializeResult ReadServerCapabilities(
            ClientCapabilities clientCapabilities, WindowClientCapabilities windowCapabilities, TextDocumentClientCapabilities textDocumentCapabilities
        )
        {
            // little hack to ensure that we get the proposed capabilities if proposals are turned on
            var serverCapabilities = _serializer.DeserializeObject<ServerCapabilities>("{}");

            var serverCapabilitiesObject = new JObject();
            foreach (var converter in _registrationOptionsConverters)
            {
                var keys = ( converter.Key ?? Array.Empty<string>() ).Select(key => char.ToLower(key[0]) + key.Substring(1)).ToArray();
                var value = serverCapabilitiesObject;
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

                var descriptor = _collection
                                .Where(z => z.HasRegistration)
                                .FirstOrDefault(z => converter.SourceType == z.RegistrationType);

                if (descriptor == null || _supportedCapabilities.AllowsDynamicRegistration(descriptor.CapabilityType)) continue;
                var registrationOptions = descriptor.RegistrationOptions;

                value[lastKey] = registrationOptions == null
                    ? JValue.CreateNull()
                    : JToken.FromObject(converter.Convert(registrationOptions), _serializer.JsonSerializer);
            }

            using (var reader = serverCapabilitiesObject.CreateReader())
            {
                _serializer.JsonSerializer.Populate(reader, serverCapabilities);
            }

            var ccp = new ClientCapabilityProvider(_collection, windowCapabilities.WorkDoneProgress.IsSupported);

            if (ccp.HasStaticHandler(textDocumentCapabilities.Synchronization))
            {
                var textDocumentSyncKind = TextDocumentSyncKind.None;
                if (_collection.ContainsHandler(typeof(IDidChangeTextDocumentHandler)))
                {
                    var kinds = _collection
                               .Select(x => x.Handler)
                               .OfType<IDidChangeTextDocumentHandler>()
                               .Select(
                                    x => ( (TextDocumentChangeRegistrationOptions?) x.GetRegistrationOptions(textDocumentCapabilities.Synchronization!, clientCapabilities) )
                                       ?.SyncKind
                                      ?? TextDocumentSyncKind.None
                                )
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
                    serverCapabilities.TextDocumentSync = new TextDocumentSyncOptions {
                        Change = textDocumentSyncKind,
                        OpenClose = _collection.ContainsHandler(typeof(IDidOpenTextDocumentHandler)) ||
                                    _collection.ContainsHandler(typeof(IDidCloseTextDocumentHandler)),
                        Save = _collection.ContainsHandler(typeof(IDidSaveTextDocumentHandler))
                            ? new SaveOptions { IncludeText = true /* TODO: Make configurable */ }
                            : new BooleanOr<SaveOptions>(false),
                        WillSave = _collection.ContainsHandler(typeof(IWillSaveTextDocumentHandler)),
                        WillSaveWaitUntil = _collection.ContainsHandler(typeof(IWillSaveWaitUntilTextDocumentHandler))
                    };
                }
            }

            // TODO: Need a call back here
            // serverCapabilities.Experimental;

            var result = ServerSettings = new InitializeResult {
                Capabilities = serverCapabilities,
                ServerInfo = _serverInfo
            };
            return result;
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask(_scheduler);

        public void Dispose()
        {
            _disposable.Dispose();
            _connection.Dispose();
        }

        public IDisposable Register(Action<ILanguageServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new LangaugeServerRegistry(_resolverContext, manager, _textDocumentIdentifiers));

            var result = manager.GetDisposable();
            if (_instanceHasStarted.Started)
            {
                LanguageServerHelpers.InitHandlers(this, result, _supportedCapabilities);
            }

            return LanguageServerHelpers.RegisterHandlers(_initializeComplete.Select(_ => System.Reactive.Unit.Default), Client, WorkDoneManager, _supportedCapabilities, result);
            //            return LanguageServerHelpers.RegisterHandlers(_initializingTask ?? _initializeComplete.ToTask(), Client, WorkDoneManager, _supportedCapabilities, result);
        }

        object IServiceProvider.GetService(Type serviceType) => Services.GetService(serviceType);
    }
}
