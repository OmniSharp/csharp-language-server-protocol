using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public class DebugAdapterClient : JsonRpcServerBase, IDebugAdapterClient
    {
        private HandlerCollection _collection;
        private List<OnClientStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private Connection _connection;
        private IServiceProvider _serviceProvider;
        private IClientProgressManager _progressManager;
        private DapReceiver _receiver;

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
            var server = (DebugAdapterClient)PreInit(options);
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
            var collection = new HandlerCollection();
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
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);
            services.AddSingleton<IDebugAdapterClient>(this);
            services.AddSingleton<RequestRouter>();
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<RequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            services.AddSingleton<IClientProgressManager, ClientProgressManager>();
            services.AddSingleton(_ => _.GetRequiredService<IClientProgressManager>() as IJsonRpcHandler);

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            _serviceProvider = serviceProvider;

            ResponseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _progressManager = _serviceProvider.GetRequiredService<IClientProgressManager>();

            _connection = new Connection(
                options.Input,
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                ResponseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.OnUnhandledException ?? (e => { }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );

            var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            _disposable.Add(_collection.Add(serviceHandlers));
            options.AddLinks(_collection);
        }

        public async Task Initialize(CancellationToken token)
        {
            RegisterCapabilities(ClientSettings);

            _connection.Open();
            var serverParams = await this.RequestInitialize(ClientSettings, token);
            _receiver.Initialized();

            await _startedDelegates.Select(@delegate =>
                    Observable.FromAsync(() => @delegate(this, serverParams, token))
                )
                .ToObservable()
                .Merge()
                .LastOrDefaultAsync()
                .ToTask(token);

            ServerSettings = serverParams;
        }

        private void RegisterCapabilities(InitializeRequestArguments capabilities)
        {
            capabilities.SupportsRunInTerminalRequest ??= _collection.ContainsHandler(typeof(IRunInTerminalHandler));
            capabilities.SupportsProgressReporting ??= _collection.ContainsHandler(typeof(IProgressHandler));
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager => _collection;
        public InitializeRequestArguments ClientSettings { get; }
        public InitializeResponse ServerSettings { get; private set; }
    }

    public class DebugAdapterClientOptions : DebugAdapterRpcOptionsBase<DebugAdapterClientOptions>, IDebugAdapterClientRegistry, IInitializeRequestArguments
    {
        internal readonly List<OnClientStartedDelegate> StartedDelegates = new List<OnClientStartedDelegate>();
        public ISerializer Serializer { get; set; } = new DapSerializer();
        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AdapterId { get; set; }
        public string Locale { get; set; }
        public bool? LinesStartAt1 { get; set; }
        public bool? ColumnsStartAt1 { get; set; }
        public string PathFormat { get; set; }
        public bool? SupportsVariableType { get; set; }
        public bool? SupportsVariablePaging { get; set; }
        public bool? SupportsRunInTerminalRequest { get; set; }
        public bool? SupportsMemoryReferences { get; set; }
        public bool? SupportsProgressReporting { get; set; }
        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, IJsonRpcHandler handler, JsonRpcHandlerOptions options) => this.AddHandler(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(method, handlerFunc, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandlers(params IJsonRpcHandler[] handlers) => this.AddHandlers(handlers);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<THandler>(Func<IServiceProvider, THandler> handlerFunc, JsonRpcHandlerOptions options) => this.AddHandler(handlerFunc, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<THandler>(THandler handler, JsonRpcHandlerOptions options) => this.AddHandler(handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<TTHandler>(JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler<TTHandler>(string method, JsonRpcHandlerOptions options) => this.AddHandler<TTHandler>(method, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(Type type, JsonRpcHandlerOptions options) => this.AddHandler(type, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.AddHandler(string method, Type type, JsonRpcHandlerOptions options) => this.AddHandler(method, type, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonRequest(string method, Func<JToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonRequest(string method, Func<JToken, CancellationToken, Task<JToken>> handler, JsonRpcHandlerOptions options) => OnJsonRequest(method, handler, options);
        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams, TResponse>(string method, Func<TParams, CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TResponse>(string method, Func<Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TResponse>(string method, Func<CancellationToken, Task<TResponse>> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnRequest<TParams>(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnRequest<TParams>(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Action<TParams, CancellationToken> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Action<JToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Func<JToken, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Func<JToken, Task> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnJsonNotification(string method, Action<JToken, CancellationToken> handler, JsonRpcHandlerOptions options) => OnJsonNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Action<TParams> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Func<TParams, CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification<TParams>(string method, Func<TParams, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Action handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Func<CancellationToken, Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);

        IDebugAdapterClientRegistry IJsonRpcHandlerRegistry<IDebugAdapterClientRegistry>.OnNotification(string method, Func<Task> handler, JsonRpcHandlerOptions options) => OnNotification(method, handler, options);
    }

    public static class DebugAdapterClientOptionsExtensions
    {

        public static DebugAdapterClientOptions WithSerializer(this DebugAdapterClientOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static DebugAdapterClientOptions WithRequestProcessIdentifier(this DebugAdapterClientOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static DebugAdapterClientOptions WithServices(this DebugAdapterClientOptions options, Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static DebugAdapterClientOptions OnStarted(this DebugAdapterClientOptions options,
            OnClientStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static DebugAdapterClientOptions ConfigureLogging(this DebugAdapterClientOptions options,
            Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static DebugAdapterClientOptions AddDefaultLoggingProvider(this DebugAdapterClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static DebugAdapterClientOptions ConfigureConfiguration(this DebugAdapterClientOptions options,
            Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }
}
