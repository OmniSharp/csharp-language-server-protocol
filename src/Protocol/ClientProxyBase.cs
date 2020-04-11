using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
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
        public void SendNotification(IRequest @params) => _responseRouter.SendNotification(@params);

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params, CancellationToken cancellationToken) => _responseRouter.SendRequest<T, TResponse>(method, @params, cancellationToken);
        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken) => _responseRouter.SendRequest(@params, cancellationToken);
        public Task SendRequest(IRequest @params, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(@params, cancellationToken);
        }

        public Task<TResponse> SendRequest<TResponse>(string method, CancellationToken cancellationToken) => _responseRouter.SendRequest<TResponse>(method, cancellationToken);

        public Task SendRequest<T>(string method, T @params, CancellationToken cancellationToken ) => _responseRouter.SendRequest(method, @params, cancellationToken);

        public TaskCompletionSource<JToken> GetRequest(long id) => _responseRouter.GetRequest(id);
    }
}
