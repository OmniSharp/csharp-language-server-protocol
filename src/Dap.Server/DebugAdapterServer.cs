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
using DryIoc;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public static class DebugAdapterServerServiceCollectionExtensions
    {
        internal static IContainer AddDebugAdapterServerInternals(this IContainer container, DebugAdapterServerOptions options, IServiceProvider outerServiceProvider)
        {
            container = container.AddDebugAdapterProtocolInternals(options);

            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => { }), reuse: Reuse.Singleton);
            }

            container.RegisterInstance<IOptionsFactory<DebugAdapterServerOptions>>(new ValueOptionsFactory<DebugAdapterServerOptions>(options));

            container.RegisterInstance(options.Capabilities);
            container.RegisterInstance(options.RequestProcessIdentifier);

            container.RegisterMany<DebugAdapterServerProgressManager>(nonPublicServiceTypes: true, reuse: Reuse.Singleton);
            container.RegisterMany<DebugAdapterServer>(serviceTypeCondition: type => type == typeof(IDebugAdapterServer) || type == typeof(DebugAdapterServer), reuse: Reuse.Singleton);

            // container.
            var providedConfiguration = options.Services.FirstOrDefault(z => z.ServiceType == typeof(IConfiguration) && z.ImplementationInstance is IConfiguration);
            container.RegisterDelegate<IConfiguration>(_ => {
                    var builder = new ConfigurationBuilder();
                    if (outerServiceProvider != null)
                    {
                        var outerConfiguration = outerServiceProvider.GetService<IConfiguration>();
                        if (outerConfiguration != null)
                        {
                            builder.AddConfiguration(outerConfiguration, false);
                        }
                    }

                    if (providedConfiguration != null)
                    {
                        builder.AddConfiguration(providedConfiguration.ImplementationInstance as IConfiguration);
                    }

                    return builder.Build();
                },
                reuse: Reuse.Singleton);

            return container;
        }

        public static IServiceCollection AddDebugAdapterServer(this IServiceCollection services, Action<DebugAdapterServerOptions> configureOptions = null)
        {
            return AddDebugAdapterServer(services, Options.DefaultName, configureOptions);
        }

        public static IServiceCollection AddDebugAdapterServer(this IServiceCollection services, string name, Action<DebugAdapterServerOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(DebugAdapterServer) || d.ServiceType == typeof(IDebugAdapterServer)))
            {
                services.RemoveAll<DebugAdapterServer>();
                services.RemoveAll<IDebugAdapterServer>();
                services.AddSingleton<IDebugAdapterServer>(_ =>
                    throw new NotSupportedException("DebugAdapterServer has been registered multiple times, you must use DebugAdapterServer instead"));
                services.AddSingleton<DebugAdapterServer>(_ =>
                    throw new NotSupportedException("DebugAdapterServer has been registered multiple times, you must use DebugAdapterServer instead"));
            }

            services
                .AddOptions()
                .AddLogging();
            services.TryAddSingleton<DebugAdapterServerResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<DebugAdapterServerResolver>().Get(name));
            services.TryAddSingleton<IDebugAdapterServer>(_ => _.GetRequiredService<DebugAdapterServerResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }

    public class DebugAdapterServerResolver : IDisposable
    {
        private readonly IOptionsMonitor<DebugAdapterServerOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, DebugAdapterServer> _servers = new ConcurrentDictionary<string, DebugAdapterServer>();

        public DebugAdapterServerResolver(IOptionsMonitor<DebugAdapterServerOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public DebugAdapterServer Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = DebugAdapterServer.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<DebugAdapterServer>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }

    public class DebugAdapterServer : JsonRpcServerBase, IDebugAdapterServer, IInitializeHandler
    {
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<OnDebugAdapterServerInitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<OnDebugAdapterServerDelegate> _initializedDelegates;
        private readonly IEnumerable<OnDebugAdapterServerStartedDelegate> _startedDelegates;
        private readonly IServiceProvider _serviceProvider;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly DapReceiver _receiver;
        private Task _initializingTask;
        private readonly ISubject<InitializeResponse> _initializeComplete = new AsyncSubject<InitializeResponse>();
        private readonly Capabilities _capabilities;

        internal static IContainer CreateContainer(DebugAdapterServerOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddDebugAdapterServerInternals(options, outerServiceProvider);

        public static DebugAdapterServer Create(DebugAdapterServerOptions options) => Create(options, null);
        public static DebugAdapterServer Create(Action<DebugAdapterServerOptions> optionsAction) => Create(optionsAction, null);

        public static DebugAdapterServer Create(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static DebugAdapterServer Create(DebugAdapterServerOptions options, IServiceProvider outerServiceProvider) =>
            CreateContainer(options, outerServiceProvider).Resolve<DebugAdapterServer>();

        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, CancellationToken cancellationToken) =>
            From(optionsAction, null, cancellationToken);

        public static Task<DebugAdapterServer> From(DebugAdapterServerOptions options, IServiceProvider outerServiceProvider) =>
            From(options, outerServiceProvider, CancellationToken.None);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider outerServiceProvider) =>
            From(optionsAction, outerServiceProvider, CancellationToken.None);

        public static Task<DebugAdapterServer> From(Action<DebugAdapterServerOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new DebugAdapterServerOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<DebugAdapterServer> From(DebugAdapterServerOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        internal DebugAdapterServer(
            Capabilities capabilities,
            DapReceiver receiver,
            DebugAdapterHandlerCollection collection,
            IEnumerable<OnDebugAdapterServerInitializeDelegate> initializeDelegates,
            IEnumerable<OnDebugAdapterServerDelegate> initializedDelegates,
            IEnumerable<OnDebugAdapterServerStartedDelegate> onServerStartedDelegates,
            IServiceProvider serviceProvider,
            IResponseRouter responseRouter,
            Connection connection,
            IDebugAdapterServerProgressManager progressManager
            ) : base(collection, responseRouter)
        {
            _capabilities = capabilities;
            _receiver = receiver;
            _collection = collection;
            _initializeDelegates = initializeDelegates;
            _initializedDelegates = initializedDelegates;
            _startedDelegates = onServerStartedDelegates;
            _serviceProvider = serviceProvider;
            _connection = connection;
            ProgressManager = progressManager;
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

        async Task<InitializeResponse> IRequestHandler<InitializeRequestArguments, InitializeResponse>.Handle(InitializeRequestArguments request,
            CancellationToken cancellationToken)
        {
            ClientSettings = request;

            await Task.WhenAll(_initializeDelegates.Select(c => c(this, request, cancellationToken)));

            _receiver.Initialized();

            var response = new InitializeResponse() {
                AdditionalModuleColumns = _capabilities.AdditionalModuleColumns,
                ExceptionBreakpointFilters = _capabilities.ExceptionBreakpointFilters,
                SupportedChecksumAlgorithms = _capabilities.SupportedChecksumAlgorithms,
                SupportsCompletionsRequest = _capabilities.SupportsCompletionsRequest ?? _collection.ContainsHandler(typeof(ICompletionsHandler)),
                SupportsConditionalBreakpoints = _capabilities.SupportsConditionalBreakpoints,
                SupportsDataBreakpoints = _capabilities.SupportsDataBreakpoints ??
                                          _collection.ContainsHandler(typeof(IDataBreakpointInfoHandler)) || _collection.ContainsHandler(typeof(ISetDataBreakpointsHandler)),
                SupportsDisassembleRequest = _capabilities.SupportsDisassembleRequest ?? _collection.ContainsHandler(typeof(IDisassembleHandler)),
                SupportsExceptionOptions = _capabilities.SupportsExceptionOptions,
                SupportsFunctionBreakpoints = _capabilities.SupportsFunctionBreakpoints ?? _collection.ContainsHandler(typeof(ISetFunctionBreakpointsHandler)),
                SupportsLogPoints = _capabilities.SupportsLogPoints,
                SupportsModulesRequest = _capabilities.SupportsModulesRequest ?? _collection.ContainsHandler(typeof(IModuleHandler)),
                SupportsRestartFrame = _capabilities.SupportsRestartFrame ?? _collection.ContainsHandler(typeof(IRestartFrameHandler)),
                SupportsRestartRequest = _capabilities.SupportsRestartRequest ?? _collection.ContainsHandler(typeof(IRestartHandler)),
                SupportsSetExpression = _capabilities.SupportsSetExpression ?? _collection.ContainsHandler(typeof(ISetExpressionHandler)),
                SupportsSetVariable = _capabilities.SupportsSetVariable ?? _collection.ContainsHandler(typeof(ISetVariableHandler)),
                SupportsStepBack = _capabilities.SupportsStepBack ??
                                   _collection.ContainsHandler(typeof(IStepBackHandler)) && _collection.ContainsHandler(typeof(IReverseContinueHandler)),
                SupportsTerminateRequest = _capabilities.SupportsTerminateRequest ?? _collection.ContainsHandler(typeof(ITerminateHandler)),
                SupportTerminateDebuggee = _capabilities.SupportTerminateDebuggee,
                SupportsConfigurationDoneRequest = _capabilities.SupportsConfigurationDoneRequest ?? _collection.ContainsHandler(typeof(IConfigurationDoneHandler)),
                SupportsEvaluateForHovers = _capabilities.SupportsEvaluateForHovers,
                SupportsExceptionInfoRequest = _capabilities.SupportsExceptionInfoRequest ?? _collection.ContainsHandler(typeof(IExceptionInfoHandler)),
                SupportsGotoTargetsRequest = _capabilities.SupportsGotoTargetsRequest ?? _collection.ContainsHandler(typeof(IGotoTargetsHandler)),
                SupportsHitConditionalBreakpoints = _capabilities.SupportsHitConditionalBreakpoints,
                SupportsLoadedSourcesRequest = _capabilities.SupportsLoadedSourcesRequest ?? _collection.ContainsHandler(typeof(ILoadedSourcesHandler)),
                SupportsReadMemoryRequest = _capabilities.SupportsReadMemoryRequest ?? _collection.ContainsHandler(typeof(IReadMemoryHandler)),
                SupportsTerminateThreadsRequest = _capabilities.SupportsTerminateThreadsRequest ?? _collection.ContainsHandler(typeof(ITerminateThreadsHandler)),
                SupportsValueFormattingOptions = _capabilities.SupportsValueFormattingOptions,
                SupportsDelayedStackTraceLoading = _capabilities.SupportsDelayedStackTraceLoading,
                SupportsStepInTargetsRequest = _capabilities.SupportsStepInTargetsRequest ?? _collection.ContainsHandler(typeof(IStepInTargetsHandler)),
                SupportsCancelRequest = _capabilities.SupportsCancelRequest ?? _collection.ContainsHandler(typeof(ICancelHandler)),
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

        public InitializeRequestArguments ClientSettings { get; private set; }
        public InitializeResponse ServerSettings { get; private set; }
        public IDebugAdapterServerProgressManager ProgressManager { get; }

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
