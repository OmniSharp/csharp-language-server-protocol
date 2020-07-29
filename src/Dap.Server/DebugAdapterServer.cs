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
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServer : JsonRpcServerBase, IDebugAdapterServer, IInitializeHandler
    {
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnServerStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly IServerProgressManager _progressManager;
        private readonly DapReceiver _receiver;
        private Task _initializingTask;
        private readonly ISubject<InitializeResponse> _initializeComplete = new AsyncSubject<InitializeResponse>();
        private readonly Capabilities _capabilities;

        public static Task<IDebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<IDebugAdapterServer> From(DebugAdapterServerOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<IDebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, CancellationToken token)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static IDebugAdapterServer PreInit(Action<DebugAdapterServerOptions> optionsAction)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<IDebugAdapterServer> From(DebugAdapterServerOptions options, CancellationToken token)
        {
            var server = (DebugAdapterServer) PreInit(options);
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
        public static IDebugAdapterServer PreInit(DebugAdapterServerOptions options)
        {
            return new DebugAdapterServer(options);
        }

        internal DebugAdapterServer(DebugAdapterServerOptions options) : base(options)
        {
            var services = options.Services;

            services.AddLogging(builder => options.LoggingBuilderAction(builder));

            _capabilities = options.Capabilities;

            var receiver = _receiver = new DapReceiver();
            var serializer = options.Serializer;
            var collection = new DebugAdapterHandlerCollection();
            services.AddSingleton<IHandlersManager>(collection);
            _collection = collection;
            _initializeDelegates = options.InitializeDelegates;
            _initializedDelegates = options.InitializedDelegates;
            _startedDelegates = options.StartedDelegates;

            services.AddSingleton<IOutputHandler>(_ => new OutputHandler(
                options.Output,
                options.Serializer,
                receiver.ShouldFilterOutput,
                _.GetService<ILogger<OutputHandler>>())
            );
            services.AddSingleton(_collection);
            services.AddSingleton(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);

            services.AddSingleton<IDebugAdapterServer>(this);
            services.AddSingleton<DebugAdapterRequestRouter>();
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<DebugAdapterRequestRouter>());
            services.AddSingleton<IResponseRouter, DapResponseRouter>();

            services.AddSingleton<IServerProgressManager, ServerProgressManager>();
            services.AddSingleton(_ => _.GetRequiredService<IServerProgressManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);

            _progressManager = serviceProvider.GetRequiredService<IServerProgressManager>();
            var requestRouter = serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>();
            var responseRouter = serviceProvider.GetRequiredService<IResponseRouter>();
            ResponseRouter = responseRouter;
            _connection = new Connection(
                options.Input,
                serviceProvider.GetRequiredService<IOutputHandler>(),
                receiver,
                options.RequestProcessIdentifier,
                serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                responseRouter,
                serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException,
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );

            _disposable.Add(_collection.Add(this));

            {
                var serviceHandlers = serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
                _collection.Add(this);
                _disposable.Add(_collection.Add(serviceHandlers));
                options.AddLinks(_collection);
            }
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

                this.SendInitialized(new InitializedEvent());
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

        async Task<InitializeResponse> IRequestHandler<InitializeRequestArguments, InitializeResponse>.Handle(InitializeRequestArguments request, CancellationToken cancellationToken)
        {
            ClientSettings = request;

            await Task.WhenAll(_initializeDelegates.Select(c => c(this, request, cancellationToken)));

            _receiver.Initialized();

            var response = new InitializeResponse() {
                AdditionalModuleColumns = _capabilities.AdditionalModuleColumns,
                ExceptionBreakpointFilters = _capabilities.ExceptionBreakpointFilters,
                SupportedChecksumAlgorithms = _capabilities.SupportedChecksumAlgorithms,
                SupportsCompletionsRequest = _capabilities.SupportsCompletionsRequest ??  _collection.ContainsHandler(typeof(ICompletionsHandler)),
                SupportsConditionalBreakpoints = _capabilities.SupportsConditionalBreakpoints,
                SupportsDataBreakpoints = _capabilities.SupportsDataBreakpoints ?? _collection.ContainsHandler(typeof(IDataBreakpointInfoHandler)) || _collection.ContainsHandler(typeof(ISetDataBreakpointsHandler)),
                SupportsDisassembleRequest = _capabilities.SupportsDisassembleRequest ??  _collection.ContainsHandler(typeof(IDisassembleHandler)),
                SupportsExceptionOptions = _capabilities.SupportsExceptionOptions,
                SupportsFunctionBreakpoints = _capabilities.SupportsFunctionBreakpoints ?? _collection.ContainsHandler(typeof(ISetFunctionBreakpointsHandler)),
                SupportsLogPoints = _capabilities.SupportsLogPoints,
                SupportsModulesRequest = _capabilities.SupportsModulesRequest ??  _collection.ContainsHandler(typeof(IModuleHandler)),
                SupportsRestartFrame = _capabilities.SupportsRestartFrame ?? _collection.ContainsHandler(typeof(IRestartFrameHandler)),
                SupportsRestartRequest = _capabilities.SupportsRestartRequest ??  _collection.ContainsHandler(typeof(IRestartHandler)),
                SupportsSetExpression = _capabilities.SupportsSetExpression ?? _collection.ContainsHandler(typeof(ISetExpressionHandler)),
                SupportsSetVariable = _capabilities.SupportsSetVariable ?? _collection.ContainsHandler(typeof(ISetVariableHandler)),
                SupportsStepBack = _capabilities.SupportsStepBack ?? _collection.ContainsHandler(typeof(IStepBackHandler)),
                SupportsTerminateRequest = _capabilities.SupportsTerminateRequest ??  _collection.ContainsHandler(typeof(ITerminateHandler)),
                SupportTerminateDebuggee = _capabilities.SupportTerminateDebuggee,
                SupportsConfigurationDoneRequest = _capabilities.SupportsConfigurationDoneRequest ??  _collection.ContainsHandler(typeof(IConfigurationDoneHandler)),
                SupportsEvaluateForHovers = _capabilities.SupportsEvaluateForHovers,
                SupportsExceptionInfoRequest = _capabilities.SupportsExceptionInfoRequest ??  _collection.ContainsHandler(typeof(IExceptionInfoHandler)),
                SupportsGotoTargetsRequest = _capabilities.SupportsGotoTargetsRequest ??  _collection.ContainsHandler(typeof(IGotoTargetsHandler)),
                SupportsHitConditionalBreakpoints = _capabilities.SupportsHitConditionalBreakpoints,
                SupportsLoadedSourcesRequest = _capabilities.SupportsLoadedSourcesRequest ??  _collection.ContainsHandler(typeof(ILoadedSourcesHandler)),
                SupportsReadMemoryRequest = _capabilities.SupportsReadMemoryRequest ??  _collection.ContainsHandler(typeof(IReadMemoryHandler)),
                SupportsTerminateThreadsRequest = _capabilities.SupportsTerminateThreadsRequest ??  _collection.ContainsHandler(typeof(ITerminateThreadsHandler)),
                SupportsValueFormattingOptions = _capabilities.SupportsValueFormattingOptions,
                SupportsDelayedStackTraceLoading = _capabilities.SupportsDelayedStackTraceLoading,
                SupportsStepInTargetsRequest = _capabilities.SupportsStepInTargetsRequest ??  _collection.ContainsHandler(typeof(IStepInTargetsHandler)),
                SupportsCancelRequest = _capabilities.SupportsCancelRequest ??  _collection.ContainsHandler(typeof(ICancelHandler)),
                SupportsClipboardContext = _capabilities.SupportsClipboardContext,
                SupportsInstructionBreakpoints = _capabilities.SupportsInstructionBreakpoints ?? _collection.ContainsHandler(typeof(ISetInstructionBreakpointsHandler)),
                SupportsSteppingGranularity = _capabilities.SupportsSteppingGranularity,
                SupportsBreakpointLocationsRequest = _capabilities.SupportsBreakpointLocationsRequest ?? _collection.ContainsHandler(typeof(IBreakpointLocationsHandler))
            };

            ServerSettings = response;

            await Task.WhenAll(_initializedDelegates.Select(c => c(this, request, response, cancellationToken)));
            _initializeComplete.OnNext(response);
            _initializeComplete.OnCompleted();

            return response;
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager => _collection;
        public InitializeRequestArguments ClientSettings { get; private set; }
        public InitializeResponse ServerSettings { get; private set; }
        public IServerProgressManager ProgressManager => _progressManager;

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }
    }
}
