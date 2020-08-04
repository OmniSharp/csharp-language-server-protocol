using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services)
        {
            services.AddSingleton<IOutputHandler>(_ => {
                var options = _.GetRequiredService<IOptions<JsonRpcServerOptions>>().Value;
                return new OutputHandler(
                    options.Output,
                    options.Serializer,
                    _.GetRequiredService<ILogger<OutputHandler>>()
                );
            });
            services.AddSingleton(_ => {
                var options = _.GetRequiredService<IOptions<JsonRpcServerOptions>>().Value;
                return new Connection(
                    options.Input,
                    new OutputHandler(
                        options.Output,
                        options.Serializer,
                        _.GetRequiredService<ILogger<OutputHandler>>()
                    ),
                    options.Receiver,
                    options.RequestProcessIdentifier,
                    _.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                    _.GetRequiredService<IResponseRouter>(),
                    _.GetRequiredService<ILoggerFactory>(),
                    options.OnUnhandledException ?? (e => { }),
                    options.CreateResponseException,
                    options.MaximumRequestTimeout,
                    options.SupportsContentModified,
                    options.Concurrency
                );
            });
            services.AddLogging().AddOptions();

            services.AddSingleton(_ => _.GetRequiredService<IOptions<JsonRpcServerOptions>>().Value.Handlers);
            services.AddSingleton(_ => _.GetRequiredService<IOptions<JsonRpcServerOptions>>().Value.Serializer);

            // _disposable = options.CompositeDisposable;
            services.AddJsonRpcMediatR();
            services.AddSingleton<JsonRpcServer>();
            services.AddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServer>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddSingleton(_ => {
                var options = _.GetRequiredService<IOptions<JsonRpcServerOptions>>().Value;
                return new HandlerCollection(options.Handlers, _);
            });
            services.AddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>());

            return services;
        }
    }

    class JsonRpcServerOptionsFactory : IOptionsFactory<JsonRpcServerOptions>
    {
        private readonly JsonRpcServerOptions _options;

        public JsonRpcServerOptionsFactory(JsonRpcServerOptions options)
        {
            _options = options;
        }
        public JsonRpcServerOptions Create(string name) => _options;
    }

    public class JsonRpcServer : JsonRpcServerBase, IJsonRpcServer
    {
        private readonly Connection _connection;
        private readonly HandlerCollection _collection;
        private readonly CompositeDisposable _disposable;
        private readonly IServiceProvider _serviceProvider;

        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, Action<IServiceCollection> configureServices) => From(optionsAction, configureServices, CancellationToken.None);

        public static async Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, Action<IServiceCollection> configureServices, CancellationToken cancellationToken)
        {
            var services = new ServiceCollection()
                .AddJsonRpcServer()
                .Configure(optionsAction);
            configureServices?.Invoke(services);

            var serviceProvider =  services.BuildServiceProvider();
                var server = serviceProvider.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cancellationToken);
            return server;
        }

        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, Action<IServiceCollection> configureServices) => From(options, configureServices, CancellationToken.None);

        public static async Task<JsonRpcServer> From(JsonRpcServerOptions options, Action<IServiceCollection> configureServices, CancellationToken cancellationToken)
        {
            var services = new ServiceCollection()
                .AddJsonRpcServer()
                .AddSingleton<IOptionsFactory<JsonRpcServerOptions>>(new JsonRpcServerOptionsFactory(options));
            configureServices?.Invoke(services);

            var serviceProvider =  services.BuildServiceProvider();
            var server = serviceProvider.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cancellationToken);
            return server;
        }

        public JsonRpcServer(
            IOptions<JsonRpcServerOptions> options,
            Connection connection,
            HandlerCollection handlerCollection,
            IServiceProvider serviceProvider,
            IResponseRouter responseRouter
        ) : base(options.Value)
        {
            _connection = connection;
            HandlersManager = _collection = handlerCollection;
            _disposable = options.Value.CompositeDisposable;
            ResponseRouter = responseRouter;
            _serviceProvider = serviceProvider;
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _connection.Open();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IDisposable Register(Action<IJsonRpcServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_collection);
            registryAction(new JsonRpcServerRegistry(_serviceProvider, manager));
            return manager.GetDisposable();
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager { get; }
    }
}
