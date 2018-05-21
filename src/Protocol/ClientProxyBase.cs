using System.Threading.Tasks;
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

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params) => _responseRouter.SendRequest<T, TResponse>(method, @params);

        public Task<TResponse> SendRequest<TResponse>(string method) => _responseRouter.SendRequest<TResponse>(method);

        public Task SendRequest<T>(string method, T @params) => _responseRouter.SendRequest(method, @params);

        public TaskCompletionSource<JToken> GetRequest(long id) => _responseRouter.GetRequest(id);
    }
}