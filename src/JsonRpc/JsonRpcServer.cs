using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Disposables;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerServiceCollectionExtensions
    {
        internal static IServiceCollection AddJsonRpcServerInternals(this IServiceCollection services, JsonRpcServerOptions options, IServiceProvider outerServiceProvider)
        {
            services.AddSingleton<IOutputHandler>(_ => new OutputHandler(
                options.Output,
                options.Serializer,
                _.GetRequiredService<ILogger<OutputHandler>>()
            ));
            services.AddSingleton(_ => new Connection(
                options.Input,
                _.GetRequiredService<IOutputHandler>(),
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
            ));
            services.AddLogging().AddOptions();
            if (outerServiceProvider != null)
            {
                services.AddSingleton(outerServiceProvider.GetRequiredService<ILoggerFactory>());
            }
            else if (options.LoggerFactory != null)
            {
                services.AddSingleton(options.LoggerFactory);
            }

            services.AddSingleton(_ => options.Handlers);
            services.AddSingleton(_ => options.Serializer);
            services.AddSingleton(_ => options.Receiver);
            services.AddSingleton(_ => options.RequestProcessIdentifier);

            // _disposable = options.CompositeDisposable;
            services.AddJsonRpcMediatR();
            services.AddSingleton<JsonRpcServer>();
            services.AddSingleton<IJsonRpcServer>(_ => _.GetRequiredService<JsonRpcServer>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddSingleton(_ => new HandlerCollection(options.Handlers, outerServiceProvider ?? _));
            services.AddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>());

            return services;
        }

        public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, string name = "Default")
        {
            services.AddSingleton(_ => {
                var options = _.GetRequiredService<IOptionsSnapshot<JsonRpcServerOptions>>().Get(name);

                var serviceProvider = new ServiceCollection()
                    .AddJsonRpcServerInternals(options, _)
                    .BuildServiceProvider();

                var server = serviceProvider.GetRequiredService<JsonRpcServer>();
                return server;
            });

            return services;
        }
    }

    internal class JsonRpcServerOptionsFactory<T> : IOptionsFactory<T> where T : JsonRpcServerOptions, new() {
        private readonly T _options;

        public JsonRpcServerOptionsFactory(T options)
        {
            _options = options;
        }

        public T Create(string name) => _options;
    }

    public class JsonRpcServer : JsonRpcServerBase, IJsonRpcServer
    {
        private readonly Connection _connection;
        private readonly HandlerCollection _collection;
        private readonly CompositeDisposable _disposable;

        public static Task<JsonRpcServer> From(JsonRpcServerOptions options) => From(options, null, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction) => From(optionsAction, null, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, CancellationToken cancellationToken) => From(options, null, null, cancellationToken);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, null, cancellationToken);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken) => From(options, null, outerServiceProvider, cancellationToken);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken) => From(optionsAction, null, outerServiceProvider, cancellationToken);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, Action<IServiceCollection> configureServices) => From(options, configureServices, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, Action<IServiceCollection> configureServices) => From(optionsAction, configureServices, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, Action<IServiceCollection> configureServices, IServiceProvider outerServiceProvider) => From(options, configureServices, outerServiceProvider, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, Action<IServiceCollection> configureServices, IServiceProvider outerServiceProvider) => From(optionsAction, configureServices, outerServiceProvider, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, Action<IServiceCollection> configureServices, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return From(options, configureServices, outerServiceProvider, cancellationToken);
        }

        public static async Task<JsonRpcServer> From(JsonRpcServerOptions options, Action<IServiceCollection> configureServices, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var services = new ServiceCollection()
                .AddJsonRpcServerInternals(options, outerServiceProvider)
                .AddSingleton<IOptionsFactory<JsonRpcServerOptions>>(new JsonRpcServerOptionsFactory<JsonRpcServerOptions>(options));
            configureServices?.Invoke(services);

            var serviceProvider = services.BuildServiceProvider();
            var server = serviceProvider.GetRequiredService<JsonRpcServer>();
            await server.Initialize(cancellationToken);
            return server;
        }

        public JsonRpcServer(
            IOptions<JsonRpcServerOptions> options,
            Connection connection,
            HandlerCollection handlerCollection,
            IResponseRouter responseRouter,
            IServiceProvider serviceProvider
        ) : base(options.Value)
        {
            _connection = connection;
            HandlersManager = _collection = handlerCollection;
            _disposable = options.Value.CompositeDisposable;
            ResponseRouter = responseRouter;
            _disposable.Add(_connection);
            if (serviceProvider is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
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
            registryAction(new JsonRpcServerRegistry(manager));
            return manager.GetDisposable();
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager { get; }
    }
}
