using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class DefaultJsonRpcServerFacade : IJsonRpcServerFacade
    {
        private readonly IResponseRouter _responseRouter;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<IHandlersManager> _handlersManager;

        public DefaultJsonRpcServerFacade(IResponseRouter requestRouter, IServiceProvider serviceProvider, Lazy<IHandlersManager> handlersManager)
        {
            _responseRouter = requestRouter;
            _serviceProvider = serviceProvider;
            _handlersManager = handlersManager;
        }

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest<Unit> request)
        {
            _responseRouter.SendNotification(request);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return _responseRouter.SendRequest(method);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(request, cancellationToken);
        }

        bool IResponseRouter.TryGetRequest(long id, [NotNullWhen(true)] out string? method, [NotNullWhen(true)] out TaskCompletionSource<JToken>? pendingTask)
        {
            return _responseRouter.TryGetRequest(id, out method, out pendingTask);
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IDisposable Register(Action<IJsonRpcServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_handlersManager.Value);
            registryAction(new JsonRpcServerRegistry(manager));
            return manager.GetDisposable();
        }
    }
}
