using System.Diagnostics.CodeAnalysis;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class JsonRpcServerBase : IResponseRouter
    {
        protected JsonRpcServerBase(IHandlersManager handlersManager, IResponseRouter responseRouter)
        {
            HandlersManager = handlersManager;
            ResponseRouter = responseRouter;
        }

        public IResponseRouter ResponseRouter { get; }
        public IHandlersManager HandlersManager { get; }

        public void SendNotification(string method)
        {
            ResponseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            ResponseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest<Unit> @params)
        {
            ResponseRouter.SendNotification(@params);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return ResponseRouter.SendRequest(@params, cancellationToken);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return ResponseRouter.SendRequest(method, @params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return ResponseRouter.SendRequest(method);
        }

        bool IResponseRouter.TryGetRequest(long id, [NotNullWhen(true)] out string? method, [NotNullWhen(true)] out TaskCompletionSource<JToken>? pendingTask)
        {
            return ResponseRouter.TryGetRequest(id, out method, out pendingTask);
        }
    }
}
