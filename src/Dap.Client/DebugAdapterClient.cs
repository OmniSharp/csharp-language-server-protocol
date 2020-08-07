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
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Client
{

    public static class DebugAdapterClientServiceCollectionExtensions
    {
        internal static IContainer AddDebugAdapterClientInternals(this IContainer container, DebugAdapterClientOptions options, IServiceProvider outerServiceProvider)
        {
            container = container.AddDebugAdapterProtocolInternals(options);

            if (options.OnUnhandledException != null)
            {
                container.RegisterInstance(options.OnUnhandledException);
            }
            else
            {
                container.RegisterDelegate(_ => new OnUnhandledExceptionHandler(e => {  }), reuse: Reuse.Singleton);
            }

            container.RegisterInstance<IOptionsFactory<DebugAdapterClientOptions>>(new ValueOptionsFactory<DebugAdapterClientOptions>(options));

            container.RegisterMany<DebugAdapterClient>(serviceTypeCondition: type => type == typeof(IDebugAdapterClient) || type == typeof(DebugAdapterClient), reuse: Reuse.Singleton);
            container.RegisterInitializer<DebugAdapterClient>((server, context) => {
                var manager = context.Resolve<IHandlersManager>();
                var descriptions = context.Resolve<IJsonRpcHandlerCollection>();
                descriptions.Populate(context, manager);
            });

            container.RegisterInstance(new InitializeRequestArguments() {
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
            });
            container.RegisterInstance(options.RequestProcessIdentifier);

            container.RegisterMany<DebugAdapterClientProgressManager>(nonPublicServiceTypes: true);
            container.RegisterMany<DebugAdapterClient>(serviceTypeCondition: type => type == typeof(IDebugAdapterClient) || type == typeof(DebugAdapterClient), reuse: Reuse.Singleton);

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

        public static IServiceCollection AddDebugAdapterClient(this IServiceCollection services, Action<DebugAdapterClientOptions> configureOptions = null)
        {
            return AddDebugAdapterClient(services, Options.DefaultName, configureOptions);
        }

        public static IServiceCollection AddDebugAdapterClient(this IServiceCollection services, string name, Action<DebugAdapterClientOptions> configureOptions = null)
        {
            // If we get called multiple times we're going to remove the default server
            // and force consumers to use the resolver.
            if (services.Any(d => d.ServiceType == typeof(DebugAdapterClient) || d.ServiceType == typeof(IDebugAdapterClient)))
            {
                services.RemoveAll<DebugAdapterClient>();
                services.RemoveAll<IDebugAdapterClient>();
                services.AddSingleton<IDebugAdapterClient>(_ =>
                    throw new NotSupportedException("DebugAdapterClient has been registered multiple times, you must use DebugAdapterClient instead"));
                services.AddSingleton<DebugAdapterClient>(_ =>
                    throw new NotSupportedException("DebugAdapterClient has been registered multiple times, you must use DebugAdapterClient instead"));
            }

            services
                .AddOptions()
                .AddLogging();
            services.TryAddSingleton<DebugAdapterClientResolver>();
            services.TryAddSingleton(_ => _.GetRequiredService<DebugAdapterClientResolver>().Get(name));
            services.TryAddSingleton<IDebugAdapterClient>(_ => _.GetRequiredService<DebugAdapterClientResolver>().Get(name));

            if (configureOptions != null)
            {
                services.Configure(name, configureOptions);
            }

            return services;
        }
    }

    public class DebugAdapterClientResolver : IDisposable
    {
        private readonly IOptionsMonitor<DebugAdapterClientOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, DebugAdapterClient> _servers = new ConcurrentDictionary<string, DebugAdapterClient>();

        public DebugAdapterClientResolver(IOptionsMonitor<DebugAdapterClientOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public DebugAdapterClient Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = DebugAdapterClient.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<DebugAdapterClient>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }

    public class DebugAdapterClient : JsonRpcServerBase, IDebugAdapterClient, IInitializedHandler
    {
        private readonly DebugAdapterSettingsBag _settingsBag;
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<OnDebugAdapterClientStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly DapReceiver _receiver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubject<InitializedEvent> _initializedComplete = new AsyncSubject<InitializedEvent>();

        internal static IContainer CreateContainer(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddDebugAdapterClientInternals(options, outerServiceProvider);

        public static DebugAdapterClient Create(DebugAdapterClientOptions options) => Create(options, null);
        public static DebugAdapterClient Create(Action<DebugAdapterClientOptions> optionsAction) => Create(optionsAction, null);
        public static DebugAdapterClient Create(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static DebugAdapterClient Create(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) => CreateContainer(options, outerServiceProvider).Resolve<DebugAdapterClient>();

        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options) => From(options, null, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);
        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) => From(options, outerServiceProvider, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider) => From(optionsAction, outerServiceProvider, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<DebugAdapterClient> From(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        internal DebugAdapterClient(
            IOptions<DebugAdapterClientOptions> options,
            InitializeRequestArguments clientSettings,
            DebugAdapterSettingsBag settingsBag,
            DebugAdapterHandlerCollection collection,
            IEnumerable<OnDebugAdapterClientStartedDelegate> onClientStartedDelegates,
            DapReceiver receiver,
            IResponseRouter responseRouter,
            IServiceProvider serviceProvider,
            IDebugAdapterClientProgressManager debugAdapterClientProgressManager,
            Connection connection
        ) : base(collection, responseRouter)
        {
            _settingsBag = settingsBag;
            ClientSettings = clientSettings;
            _collection = collection;
            _startedDelegates = onClientStartedDelegates;
            _receiver = receiver;
            _serviceProvider = serviceProvider;
            ProgressManager = debugAdapterClientProgressManager;
            _connection = connection;
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

        public InitializeRequestArguments ClientSettings
        {
            get => _settingsBag.ClientSettings;
            private set => _settingsBag.ClientSettings = value;
        }

        public InitializeResponse ServerSettings
        {
            get => _settingsBag.ServerSettings;
            private set => _settingsBag.ServerSettings = value;
        }
        public IDebugAdapterClientProgressManager ProgressManager { get; }

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
