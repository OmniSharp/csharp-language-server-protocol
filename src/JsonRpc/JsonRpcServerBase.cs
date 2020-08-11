using System.Threading;
using System.Threading.Tasks;
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

        public void SendNotification(string method) => ResponseRouter.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => ResponseRouter.SendNotification(method, @params);

        public void SendNotification(IRequest @params) => ResponseRouter.SendNotification(@params);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken) => ResponseRouter.SendRequest(@params, cancellationToken);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => ResponseRouter.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => ResponseRouter.SendRequest(method);

        (string method, TaskCompletionSource<JToken> pendingTask) IResponseRouter.GetRequest(long id) => ResponseRouter.GetRequest(id);
    }
}
