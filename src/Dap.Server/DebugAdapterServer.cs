using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
// ReSharper disable SuspiciousTypeConversion.Global

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServer : JsonRpcServerBase, IDebugAdapterServer, IDebugAdapterInitializeHandler
    {
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<OnDebugAdapterServerInitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<IOnDebugAdapterServerInitialize> _initializeHandlers;
        private readonly IEnumerable<OnDebugAdapterServerInitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<IOnDebugAdapterServerInitialized> _initializedHandlers;
        private readonly IEnumerable<OnDebugAdapterServerStartedDelegate> _startedDelegates;
        private readonly IEnumerable<IOnDebugAdapterServerStarted> _startedHandlers;
        private readonly IServiceProvider _serviceProvider;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly DapReceiver _receiver;
        private Task? _initializingTask;
        private readonly ISubject<InitializeResponse> _initializeComplete = new AsyncSubject<InitializeResponse>();
        private readonly Capabilities _capabilities;
        private readonly DebugAdapterSettingsBag _settingsBag;
        private readonly int? _concurrency;

        internal static IContainer CreateContainer(DebugAdapterServerOptions options, IServiceProvider? outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                                  .AddDebugAdapterServerInternals(options, outerServiceProvider);

        public static DebugAdapterServer Create(DebugAdapterServerOptions options) => Create(options, null);
        public static DebugAdapterServer Create(Action<DebugAdapterServerOptions> optionsAction) => Create(optionsAction, null);

        public static DebugAdapterServer Create(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider? outerServiceProvider)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static DebugAdapterServer Create(DebugAdapterServerOptions options, IServiceProvider? outerServiceProvider) =>
            CreateContainer(options, outerServiceProvider).Resolve<DebugAdapterServer>();

        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, CancellationToken cancellationToken) =>
            From(optionsAction, null, cancellationToken);

        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options, IServiceProvider? outerServiceProvider) =>
            From(options, outerServiceProvider, CancellationToken.None);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider? outerServiceProvider) =>
            From(optionsAction, outerServiceProvider, CancellationToken.None);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<DebugAdapterServer> From(DebugAdapterServerOptions options, IServiceProvider? outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        internal DebugAdapterServer(
            IOptions<DebugAdapterServerOptions> options,
            Capabilities capabilities,
            DebugAdapterSettingsBag settingsBag,
            DapReceiver receiver,
            DebugAdapterHandlerCollection collection,
            IEnumerable<OnDebugAdapterServerInitializeDelegate> initializeDelegates,
            IEnumerable<OnDebugAdapterServerInitializedDelegate> initializedDelegates,
            IEnumerable<OnDebugAdapterServerStartedDelegate> onServerStartedDelegates,
            IServiceProvider serviceProvider,
            IResponseRouter responseRouter,
            Connection connection,
            IDebugAdapterServerProgressManager progressManager,
            IEnumerable<IOnDebugAdapterServerInitialize> initializeHandlers,
            IEnumerable<IOnDebugAdapterServerInitialized> initializedHandlers,
            IEnumerable<IOnDebugAdapterServerStarted> startedHandlers
        ) : base(collection, responseRouter)
        {
            _capabilities = capabilities;
            _settingsBag = settingsBag;
            _receiver = receiver;
            _collection = collection;
            _initializeDelegates = initializeDelegates;
            _initializedDelegates = initializedDelegates;
            _startedDelegates = onServerStartedDelegates;
            _serviceProvider = serviceProvider;
            _connection = connection;
            ProgressManager = progressManager;
            _initializeHandlers = initializeHandlers;
            _initializedHandlers = initializedHandlers;
            _startedHandlers = startedHandlers;
            _concurrency = options.Value.Concurrency;

            _disposable.Add(collection.Add(this));
        }


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
                _initializingTask = _initializeComplete.ToTask(token);
                await _initializingTask;
                await DebugAdapterEventingHelper.Run(
                    _startedDelegates,
                    (handler, ct) => handler(this, ct),
                    _startedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnDebugAdapterServerStarted>()),
                    (handler, ct) => handler.OnStarted(this, ct),
                    _concurrency,
                    token
                );

                this.SendDebugAdapterInitialized(new InitializedEvent());
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

        async Task<InitializeResponse> IRequestHandler<InitializeRequestArguments, InitializeResponse>.Handle(
            InitializeRequestArguments request,
            CancellationToken cancellationToken
        )
        {
            _settingsBag.ClientSettings = request;

            await DebugAdapterEventingHelper.Run(
                _initializeDelegates,
                (handler, ct) => handler(this, request, ct),
                _initializeHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnDebugAdapterServerInitialize>()),
                (handler, ct) => handler.OnInitialize(this, request, ct),
                _concurrency,
                cancellationToken
            );

            _receiver.Initialized();

            var response = new InitializeResponse {
                AdditionalModuleColumns = _capabilities.AdditionalModuleColumns,
                ExceptionBreakpointFilters = _capabilities.ExceptionBreakpointFilters,
                SupportedChecksumAlgorithms = _capabilities.SupportedChecksumAlgorithms,
                SupportsCompletionsRequest = _capabilities.SupportsCompletionsRequest || _collection.ContainsHandler(typeof(ICompletionsHandler)),
                SupportsConditionalBreakpoints = _capabilities.SupportsConditionalBreakpoints,
                SupportsDataBreakpoints = _capabilities.SupportsDataBreakpoints ||
                                          _collection.ContainsHandler(typeof(IDataBreakpointInfoHandler)) || _collection.ContainsHandler(typeof(ISetDataBreakpointsHandler)),
                SupportsDisassembleRequest = _capabilities.SupportsDisassembleRequest || _collection.ContainsHandler(typeof(IDisassembleHandler)),
                SupportsExceptionOptions = _capabilities.SupportsExceptionOptions,
                SupportsFunctionBreakpoints = _capabilities.SupportsFunctionBreakpoints || _collection.ContainsHandler(typeof(ISetFunctionBreakpointsHandler)),
                SupportsLogPoints = _capabilities.SupportsLogPoints,
                SupportsModulesRequest = _capabilities.SupportsModulesRequest || _collection.ContainsHandler(typeof(IModuleHandler)),
                SupportsRestartFrame = _capabilities.SupportsRestartFrame || _collection.ContainsHandler(typeof(IRestartFrameHandler)),
                SupportsRestartRequest = _capabilities.SupportsRestartRequest || _collection.ContainsHandler(typeof(IRestartHandler)),
                SupportsSetExpression = _capabilities.SupportsSetExpression || _collection.ContainsHandler(typeof(ISetExpressionHandler)),
                SupportsSetVariable = _capabilities.SupportsSetVariable || _collection.ContainsHandler(typeof(ISetVariableHandler)),
                SupportsStepBack = _capabilities.SupportsStepBack ||
                                   _collection.ContainsHandler(typeof(IStepBackHandler)) && _collection.ContainsHandler(typeof(IReverseContinueHandler)),
                SupportsTerminateRequest = _capabilities.SupportsTerminateRequest || _collection.ContainsHandler(typeof(ITerminateHandler)),
                SupportTerminateDebuggee = _capabilities.SupportTerminateDebuggee,
                SupportsConfigurationDoneRequest = _capabilities.SupportsConfigurationDoneRequest || _collection.ContainsHandler(typeof(IConfigurationDoneHandler)),
                SupportsEvaluateForHovers = _capabilities.SupportsEvaluateForHovers,
                SupportsExceptionInfoRequest = _capabilities.SupportsExceptionInfoRequest || _collection.ContainsHandler(typeof(IExceptionInfoHandler)),
                SupportsGotoTargetsRequest = _capabilities.SupportsGotoTargetsRequest || _collection.ContainsHandler(typeof(IGotoTargetsHandler)),
                SupportsHitConditionalBreakpoints = _capabilities.SupportsHitConditionalBreakpoints,
                SupportsLoadedSourcesRequest = _capabilities.SupportsLoadedSourcesRequest || _collection.ContainsHandler(typeof(ILoadedSourcesHandler)),
                SupportsReadMemoryRequest = _capabilities.SupportsReadMemoryRequest || _collection.ContainsHandler(typeof(IReadMemoryHandler)),
                SupportsTerminateThreadsRequest = _capabilities.SupportsTerminateThreadsRequest || _collection.ContainsHandler(typeof(ITerminateThreadsHandler)),
                SupportsValueFormattingOptions = _capabilities.SupportsValueFormattingOptions,
                SupportsDelayedStackTraceLoading = _capabilities.SupportsDelayedStackTraceLoading,
                SupportsStepInTargetsRequest = _capabilities.SupportsStepInTargetsRequest || _collection.ContainsHandler(typeof(IStepInTargetsHandler)),
                SupportsCancelRequest = _capabilities.SupportsCancelRequest || _collection.ContainsHandler(typeof(ICancelHandler)),
                SupportsClipboardContext = _capabilities.SupportsClipboardContext,
                SupportsInstructionBreakpoints = _capabilities.SupportsInstructionBreakpoints || _collection.ContainsHandler(typeof(ISetInstructionBreakpointsHandler)),
                SupportsSteppingGranularity = _capabilities.SupportsSteppingGranularity,
                SupportsBreakpointLocationsRequest = _capabilities.SupportsBreakpointLocationsRequest || _collection.ContainsHandler(typeof(IBreakpointLocationsHandler))
            };

            _settingsBag.ServerSettings = response;

            await DebugAdapterEventingHelper.Run(
                _initializedDelegates,
                (handler, ct) => handler(this, request, response, ct),
                _initializedHandlers.Union(_collection.Select(z => z.Handler).OfType<IOnDebugAdapterServerInitialized>()),
                (handler, ct) => handler.OnInitialized(this, request, response, ct),
                _concurrency,
                cancellationToken
            );

            _initializeComplete.OnNext(response);
            _initializeComplete.OnCompleted();

            return response;
        }

        public InitializeRequestArguments ClientSettings => _settingsBag.ClientSettings;
        public InitializeResponse ServerSettings => _settingsBag.ServerSettings;
        public IDebugAdapterServerProgressManager ProgressManager { get; }

        public void Dispose()
        {
            _disposable.Dispose();
            _connection.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
