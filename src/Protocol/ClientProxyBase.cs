using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public abstract class ClientProxyBase : IResponseRouter
    {
        private readonly IResponseRouter _responseRouter;
        public ClientProxyBase(IResponseRouter responseRouter)
        {
            _responseRouter = responseRouter;
        }

        public void SendNotification(string method) => _responseRouter.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => _responseRouter.SendNotification(method, @params);

        public void SendNotification(IRequest request) => _responseRouter.SendNotification(request);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => _responseRouter.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => _responseRouter.SendRequest(method);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => _responseRouter.SendRequest(request, cancellationToken);

        public TaskCompletionSource<JToken> GetRequest(long id) => _responseRouter.GetRequest(id);
    }
}
