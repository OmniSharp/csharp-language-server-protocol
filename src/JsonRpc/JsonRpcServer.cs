using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Disposables;
using System.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.JsonRpc.DryIoc;
using DryIoc;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServer : JsonRpcServerBase, IJsonRpcServer
    {
        private readonly Connection _connection;
        private readonly CompositeDisposable _disposable;

        internal static IContainer CreateContainer(JsonRpcServerOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddJsonRpcServerInternals(options);

        public static JsonRpcServer Create(JsonRpcServerOptions options) => Create(options, null);
        public static JsonRpcServer Create(Action<JsonRpcServerOptions> optionsAction) => Create(optionsAction, null);
        public static JsonRpcServer Create(Action<JsonRpcServerOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static JsonRpcServer Create(JsonRpcServerOptions options, IServiceProvider outerServiceProvider) => CreateContainer(options, outerServiceProvider).Resolve<JsonRpcServer>();

        public static Task<JsonRpcServer> From(JsonRpcServerOptions options) => From(options, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);
        public static Task<JsonRpcServer> From(JsonRpcServerOptions options, IServiceProvider outerServiceProvider) => From(options, outerServiceProvider, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, IServiceProvider outerServiceProvider) => From(optionsAction, outerServiceProvider, CancellationToken.None);
        public static Task<JsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<JsonRpcServer> From(JsonRpcServerOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        internal JsonRpcServer(
            IOptions<JsonRpcServerOptions> options,
            Connection connection,
            IHandlersManager handlerCollection,
            IResponseRouter responseRouter,
            IServiceProvider serviceProvider
        ) : base(options.Value)
        {
            _connection = connection;
            HandlersManager = handlerCollection;
            _disposable = options.Value.CompositeDisposable;
            ResponseRouter = responseRouter;
            _disposable.Add(_connection);
            if (serviceProvider is IDisposable disposable)
            {
                _disposable.Add(disposable);
            }
        }

        public virtual async Task Initialize(CancellationToken cancellationToken)
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
            var manager = new CompositeHandlersManager(HandlersManager);
            registryAction(new JsonRpcServerRegistry(manager));
            return manager.GetDisposable();
        }

        protected override IResponseRouter ResponseRouter { get; }
        protected override IHandlersManager HandlersManager { get; }
    }
}
