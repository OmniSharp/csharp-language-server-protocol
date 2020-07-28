using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServer : JsonRpcServerBase, IDebugAdapterServer, IDisposable, IInitializeHandler
    {
        private HandlerCollection _collection;
        private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnServerStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private Connection _connection;
        private IServiceProvider _serviceProvider;
        private IServerProgressManager _progressManager;
        private DapReceiver _receiver;
        private Task _initializingTask;
        private readonly ISubject<InitializeResponse> _initializeComplete = new AsyncSubject<InitializeResponse>();
        private Capabilities _capabilities;
        private ISerializer _serializer;

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
            _serializer = options.Serializer;
            var collection = new HandlerCollection();
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
            services.AddSingleton(_serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);

            services.AddSingleton<IDebugAdapterServer>(this);
            services.AddSingleton<RequestRouter>();
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<RequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            services.AddSingleton<IServerProgressManager, ServerProgressManager>();
            services.AddSingleton(_ => _.GetRequiredService<IServerProgressManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            _serviceProvider = serviceProvider;

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>();
            var responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            ResponseRouter = responseRouter;
            _connection = new Connection(
                options.Input,
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                responseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException,
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );

            _disposable.Add(_collection.Add(this));

            {
                var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
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

            await Task.WhenAll(_initializedDelegates.Select(c => c(this, request, response, cancellationToken)));
            _initializeComplete.OnNext(response);
            _initializeComplete.OnCompleted();
            return response;
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager => _collection;
        public InitializeRequestArguments ClientSettings { get; }
        public InitializeResponse ServerSettings { get; private set; }
        public IServerProgressManager ProgressManager => _progressManager;

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
            _initializingTask?.Dispose();
        }
    }

    public class DebugAdapterServerOptions : DebugAdapterRpcOptionsBase<DebugAdapterServerOptions>, IDebugAdapterServerRegistry
    {
        public Capabilities Capabilities { get; set; } = new Capabilities();
        internal readonly List<OnServerStartedDelegate> StartedDelegates = new List<OnServerStartedDelegate>();
        internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) =>
            this.AddHandler(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc,
            JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc,
            JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) =>
            this.AddHandler(handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) =>
            this.AddHandler<TTHandler>(options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) =>
            this.AddHandler<TTHandler>(method, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) =>
            this.AddHandler(method, type, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler,
            JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler,
            JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams, TResponse>(string method,
            Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) =>
            OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler,
            JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Action<TParams> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler,
            JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterServerRegistry IJsonRpcHandlerRegistry<IDebugAdapterServerRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) =>
            OnNotification(method, handler, options);
    }

    public delegate Task InitializedDelegate(IDebugAdapterServer server, InitializeRequestArguments request, InitializeResponse response, CancellationToken cancellationToken);

    public delegate Task InitializeDelegate(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);

    public static class DebugAdapterServerOptionsExtensions
    {
        public static DebugAdapterServerOptions WithSerializer(this DebugAdapterServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static DebugAdapterServerOptions WithRequestProcessIdentifier(this DebugAdapterServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static DebugAdapterServerOptions WithServices(this DebugAdapterServerOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static DebugAdapterServerOptions OnInitialize(this DebugAdapterServerOptions options, InitializeDelegate @delegate)
        {
            options.InitializeDelegates.Add(@delegate);
            return options;
        }


        public static DebugAdapterServerOptions OnInitialized(this DebugAdapterServerOptions options, InitializedDelegate @delegate)
        {
            options.InitializedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions OnStarted(this DebugAdapterServerOptions options, OnServerStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterServerOptions ConfigureLogging(this DebugAdapterServerOptions options, Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterServerOptions AddDefaultLoggingProvider(this DebugAdapterServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterServerOptions ConfigureConfiguration(this DebugAdapterServerOptions options, Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }


    public interface IServerProgressManager
    {
        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null);
    }

    public interface IProgressObserver : IObserver<ProgressUpdateEvent>, IDisposable
    {
        ProgressToken ProgressId { get; }
        void OnNext(string message, double? percentage);
    }


    public class ServerProgressManager : ICancelHandler, IServerProgressManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;

        private readonly ConcurrentDictionary<ProgressToken, CancellationTokenSource> _activeObserverTokens
            = new ConcurrentDictionary<ProgressToken, CancellationTokenSource>(EqualityComparer<ProgressToken>.Default);

        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers =
            new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);

        public ServerProgressManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        public IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null)
        {
            if (EqualityComparer<ProgressToken>.Default.Equals(begin.ProgressId, default))
            {
                begin.ProgressId = new ProgressToken(Guid.NewGuid().ToString());
            }

            if (_activeObservers.TryGetValue(begin.ProgressId, out var item))
            {
                return item;
            }

            onError ??= error => new ProgressEndEvent() {
                Message = error.ToString()
            };

            onComplete ??= () => new ProgressEndEvent();

            var cts = new CancellationTokenSource();
            var observer = new ProgressObserver(
                _router,
                _serializer,
                begin,
                onError,
                onComplete,
                cts.Token
            );
            _activeObservers.TryAdd(observer.ProgressId, observer);
            _activeObserverTokens.TryAdd(observer.ProgressId, cts);

            return observer;
        }

        public Task<CancelResponse> Handle(CancelArguments request, CancellationToken cancellationToken)
        {
            if (request.ProgressId.HasValue && _activeObserverTokens.TryGetValue(request.ProgressId.Value, out var cts))
            {
                cts.Cancel();
            }

            return Task.FromResult(new CancelResponse());
        }
    }

    class ProgressObserver : IProgressObserver
    {
        private readonly ProgressToken _progressToken;
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly Func<Exception, ProgressEndEvent> _onError;
        private readonly Func<ProgressEndEvent> _onComplete;
        private readonly CompositeDisposable _disposable;

        public ProgressObserver(
            IResponseRouter router,
            ISerializer serializer,
            ProgressStartEvent begin,
            Func<Exception, ProgressEndEvent> onError,
            Func<ProgressEndEvent> onComplete,
            CancellationToken cancellationToken)
        {
            _progressToken = begin.ProgressId;
            _router = router;
            _serializer = serializer;
            _onError = onError;
            _onComplete = onComplete;
            _disposable = new CompositeDisposable {Disposable.Create(OnCompleted)};
            cancellationToken.Register(Dispose);
            _router.SendNotification(begin);
        }

        public void OnCompleted()
        {
            var @event = _onComplete?.Invoke() ?? new ProgressEndEvent() {Message = "", ProgressId = _progressToken};
            if (EqualityComparer<ProgressToken>.Default.Equals(@event.ProgressId, default))
            {
                @event.ProgressId = _progressToken;
            }

            _router.SendNotification(@event);
        }

        void IObserver<ProgressUpdateEvent>.OnError(Exception error)
        {
            var @event = _onError?.Invoke(error) ?? new ProgressEndEvent() {Message = error.ToString(), ProgressId = _progressToken};
            if (EqualityComparer<ProgressToken>.Default.Equals(@event.ProgressId, default))
            {
                @event.ProgressId = _progressToken;
            }

            _router.SendNotification(@event);
        }

        public void OnNext(ProgressUpdateEvent value)
        {
            if (EqualityComparer<ProgressToken>.Default.Equals(value.ProgressId, default))
            {
                value.ProgressId = _progressToken;
            }

            _router.SendNotification(value);
        }

        public ProgressToken ProgressId => _progressToken;

        public void OnNext(string message, double? percentage)
        {
            OnNext(new ProgressUpdateEvent() {
                ProgressId = _progressToken,
                Message = message,
                Percentage = percentage
            });
        }

        public void Dispose() => _disposable?.Dispose();
    }
}
