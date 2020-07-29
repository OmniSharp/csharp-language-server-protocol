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
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public class DebugAdapterClient : JsonRpcServerBase, IDebugAdapterClient, IInitializedHandler
    {
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<OnClientStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly IClientProgressManager _progressManager;
        private readonly DapReceiver _receiver;
        private readonly ISubject<InitializedEvent> _initializedComplete = new AsyncSubject<InitializedEvent>();

        public static Task<IDebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<IDebugAdapterClient> From(DebugAdapterClientOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<IDebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, CancellationToken token)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static IDebugAdapterClient PreInit(Action<DebugAdapterClientOptions> optionsAction)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<IDebugAdapterClient> From(DebugAdapterClientOptions options, CancellationToken token)
        {
            var server = (DebugAdapterClient) PreInit(options);
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
        public static IDebugAdapterClient PreInit(DebugAdapterClientOptions options)
        {
            return new DebugAdapterClient(options);
        }

        internal DebugAdapterClient(DebugAdapterClientOptions options) : base(options)
        {
            var services = options.Services;
            services.AddLogging(builder => options.LoggingBuilderAction(builder));

            ClientSettings = new InitializeRequestArguments() {
                Locale = options.Locale,
                AdapterId = options.AdapterId,
                ClientId = options.ClientId,
                ClientName = options.ClientName,
                PathFormat = options.PathFormat,
                ColumnsStartAt1 = options.ColumnsStartAt1,
                LinesStartAt1 = options.LinesStartAt1,
                SupportsMemoryReferences = options.SupportsMemoryReferences,
                SupportsProgressReporting = options.SupportsProgressReporting,
                SupportsVariablePaging = options.SupportsVariablePaging,
                SupportsVariableType = options.SupportsVariableType,
                SupportsRunInTerminalRequest = options.SupportsRunInTerminalRequest,
            };

            var serializer = options.Serializer;
            var collection = new DebugAdapterHandlerCollection();
            services.AddSingleton<IHandlersManager>(collection);
            _collection = collection;
            // _initializeDelegates = initializeDelegates;
            // _initializedDelegates = initializedDelegates;
            _startedDelegates = options.StartedDelegates;

            var receiver = _receiver = new DapReceiver();

            services.AddSingleton<IOutputHandler>(_ =>
                new OutputHandler(options.Output, options.Serializer, receiver.ShouldFilterOutput, _.GetService<ILogger<OutputHandler>>()));
            services.AddSingleton(_collection);
            services.AddSingleton(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);
            services.AddSingleton<IDebugAdapterClient>(this);
            services.AddSingleton<DebugAdapterRequestRouter>();
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<DebugAdapterRequestRouter>());
            services.AddSingleton<IResponseRouter, DapResponseRouter>();

            services.AddSingleton<IClientProgressManager, ClientProgressManager>();
            services.AddSingleton(_ => _.GetRequiredService<IClientProgressManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            IServiceProvider serviceProvider1 = serviceProvider;

            var responseRouter = serviceProvider1.GetRequiredService<IResponseRouter>();
            ResponseRouter = responseRouter;
            _progressManager = serviceProvider1.GetRequiredService<IClientProgressManager>();

            _connection = new Connection(
                options.Input,
                serviceProvider1.GetRequiredService<IOutputHandler>(),
                receiver,
                options.RequestProcessIdentifier,
                serviceProvider1.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                responseRouter,
                serviceProvider1.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException ?? (e => { }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                false,
                options.Concurrency
            );

            var serviceHandlers = serviceProvider1.GetServices<IJsonRpcHandler>().ToArray();
            _collection.Add(this);
            _disposable.Add(_collection.Add(serviceHandlers));
            options.AddLinks(_collection);
        }

        public async Task Initialize(CancellationToken token)
        {
            RegisterCapabilities(ClientSettings);

            _connection.Open();
            var serverParams = await this.RequestInitialize(ClientSettings, token);

            ServerSettings = serverParams;
            _receiver.Initialized();

            await _initializedComplete.ToTask(token);

            await _startedDelegates.Select(@delegate => Observable.FromAsync(() => @delegate(this, serverParams, token)))
                .ToObservable()
                .Merge()
                .LastOrDefaultAsync()
                .ToTask(token);
        }

        Task<Unit> IRequestHandler<InitializedEvent, Unit>.Handle(InitializedEvent request, CancellationToken cancellationToken)
        {
            _initializedComplete.OnNext(request);
            _initializedComplete.OnCompleted();
            return Unit.Task;
        }

        private void RegisterCapabilities(InitializeRequestArguments capabilities)
        {
            capabilities.SupportsRunInTerminalRequest ??= _collection.ContainsHandler(typeof(IRunInTerminalHandler));
            capabilities.SupportsProgressReporting ??= _collection.ContainsHandler(typeof(IProgressStartHandler)) &&
                                                       _collection.ContainsHandler(typeof(IProgressUpdateHandler)) &&
                                                       _collection.ContainsHandler(typeof(IProgressEndHandler));
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager => _collection;
        public InitializeRequestArguments ClientSettings { get; }
        public InitializeResponse ServerSettings { get; private set; }
        public IClientProgressManager ProgressManager => _progressManager;

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }
    }
}
