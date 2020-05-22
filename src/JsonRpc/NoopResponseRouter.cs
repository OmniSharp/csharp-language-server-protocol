using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// Used top ensure noop behaviors don't throw if they consume a router
    /// </summary>
    class NoopResponseRouter : IResponseRouter
    {
        private NoopResponseRouter() {}
        public static NoopResponseRouter Instance = new NoopResponseRouter();

        public void SendNotification(string method)
        {

        }

        public void SendNotification<T>(string method, T @params)
        {

        }

        public void SendNotification(IRequest request)
        {

        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => new Impl();

        public IResponseRouterReturns SendRequest(string method) => new Impl();

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            return Task.FromResult<TResponse>(default);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return new TaskCompletionSource<JToken>();
        }

        class Impl : IResponseRouterReturns
        {
            public Task<TResponse> Returning<TResponse>(CancellationToken cancellationToken)
            {
                return Task.FromResult<TResponse>(default);
            }

            public Task ReturningVoid(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}