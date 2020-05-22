using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Reactive.Disposables;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServer : IJsonRpcServer
    {
        private readonly Connection _connection;
        private readonly HandlerCollection _collection;

        private readonly List<(string method, Func<IServiceProvider, IJsonRpcHandler>)> _namedHandlers =
            new List<(string method, Func<IServiceProvider, IJsonRpcHandler>)>();

        private readonly IResponseRouter _responseRouter;
        private readonly IServiceProvider _serviceProvider;

        public static Task<IJsonRpcServer> From(Action<JsonRpcServerOptions> optionsAction)
        {
            var options = new JsonRpcServerOptions();
            optionsAction(options);
            return From(options);
        }

        public static async Task<IJsonRpcServer> From(JsonRpcServerOptions options)
        {
            var server = new JsonRpcServer(options);

            await server.Initialize();

            return server;
        }

        internal JsonRpcServer(JsonRpcServerOptions options)
        {
            var outputHandler = new OutputHandler(options.Output, options.Serializer,
                options.LoggerFactory.CreateLogger<OutputHandler>());
            var services = options.Services;
            services.AddLogging();
            var receiver = options.Receiver;
            var serializer = options.Serializer;
            _collection = new HandlerCollection();

            services.AddSingleton<IOutputHandler>(outputHandler);
            services.AddSingleton(_collection);
            services.AddSingleton(serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton(receiver);
            services.AddSingleton(options.LoggerFactory);

            services.AddJsonRpcMediatR(options.HandlerAssemblies);
            services.AddSingleton<IJsonRpcServer>(this);
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>, RequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            var foundHandlers = services
                .Where(x => typeof(IJsonRpcHandler).IsAssignableFrom(x.ServiceType) &&
                            x.ServiceType != typeof(IJsonRpcHandler))
                .ToArray();

            // Handlers are created at the start and maintained as a singleton
            foreach (var handler in foundHandlers)
            {
                services.Remove(handler);

                if (handler.ImplementationFactory != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationFactory));
                else if (handler.ImplementationInstance != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationInstance));
                else
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationType));
            }

            _serviceProvider = services.BuildServiceProvider();

            var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            _collection.Add(serviceHandlers);
            _collection.Add(options.Handlers.ToArray());
            foreach (var (name, handler) in options.NamedHandlers)
            {
                _collection.Add(name, handler);
            }

            foreach (var (name, handlerFunc) in options.NamedServiceHandlers)
            {
                _collection.Add(name, handlerFunc(_serviceProvider));
            }

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>();
            _collection.Add(new CancelRequestHandler<IHandlerDescriptor>(requestRouter));
            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _connection = new Connection(
                options.Input,
                outputHandler,
                options.Receiver,
                options.RequestProcessIdentifier,
                requestRouter,
                _responseRouter,
                options.LoggerFactory,
                options.OnServerError,
                options.SupportsContentModified,
                options.Concurrency
            );
        }

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            return _collection.Add(method, handler);
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            _namedHandlers.Add((method, handlerFunc));
            return Disposable.Empty;
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            return _collection.Add(handlers);
        }

        public IDisposable AddHandler(string method, Type handlerType)
        {
            return _collection.Add(method,
                ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler);
        }

        public IDisposable AddHandler<T>()
            where T : IJsonRpcHandler
        {
            return AddHandlers(typeof(T));
        }

        public IDisposable AddHandlers(params Type[] handlerTypes)
        {
            return _collection.Add(
                handlerTypes
                    .Select(handlerType =>
                        ActivatorUtilities.CreateInstance(_serviceProvider, handlerType) as IJsonRpcHandler)
                    .ToArray());
        }

        private async Task Initialize()
        {
            await Task.Yield();
            _connection.Open();
        }

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest @params)
        {
            _responseRouter.SendNotification(@params);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(@params, cancellationToken);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return _responseRouter.SendRequest(method);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
