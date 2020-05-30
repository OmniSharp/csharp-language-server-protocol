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

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServer : JsonRpcServerBase, IJsonRpcServer, IDisposable
    {
        private readonly Connection _connection;
        private readonly HandlerCollection _collection;
        private readonly CompositeDisposable _disposable;
        private readonly IServiceProvider _serviceProvider;

        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, CancellationToken cancellationToken)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return From(options, cancellationToken);
        }
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return From(options, CancellationToken.None);
        }

        public static async Task<JsonRpcServer> From(JsonRpcServerOptions options, CancellationToken cancellationToken)
        {
            var server = new JsonRpcServer(options);

            await server.Initialize(cancellationToken);

            return server;
        }

        public static Task<JsonRpcServer> From(JsonRpcServerOptions options)
        {
            return From(options, CancellationToken.None);
        }

        internal JsonRpcServer(JsonRpcServerOptions options) : base(options)
        {
            var outputHandler = new OutputHandler(
                options.Output,
                options.Serializer,
                options.LoggerFactory.CreateLogger<OutputHandler>()
            );
            var services = options.Services;
            services.AddLogging();
            var receiver = options.Receiver;
            var serializer = options.Serializer;
            _disposable = options.CompositeDisposable;

            services.AddSingleton<IOutputHandler>(outputHandler);
            services.AddSingleton(serializer);
            services.AddSingleton(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);
            services.AddSingleton(options.LoggerFactory);

            services.AddJsonRpcMediatR(options.Assemblies);
            services.AddSingleton<IJsonRpcServer>(this);
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            _collection = new HandlerCollection();
            services.AddSingleton(_collection);
            services.AddSingleton<IHandlersManager>(_ => _.GetRequiredService<HandlerCollection>());

            EnsureAllHandlersAreRegistered();

            var serviceProvider = services.BuildServiceProvider();
            _disposable.Add(serviceProvider);
            _serviceProvider = serviceProvider;
            HandlersManager = _collection;

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>();
            var router = ResponseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _connection = new Connection(
                options.Input,
                outputHandler,
                options.Receiver,
                options.RequestProcessIdentifier,
                requestRouter,
                router,
                options.LoggerFactory,
                options.OnUnhandledException ?? (e => { }),
                options.CreateResponseException,
                options.MaximumRequestTimeout,
                options.SupportsContentModified,
                options.Concurrency
            );
            _disposable.Add(_connection);
            _collection.Add(_serviceProvider.GetRequiredService<IEnumerable<IJsonRpcHandler>>().ToArray());
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
