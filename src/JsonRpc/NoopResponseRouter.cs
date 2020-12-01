using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// Used top ensure noop behaviors don't throw if they consume a router
    /// </summary>
    internal class NoopResponseRouter : IResponseRouter
    {
        private NoopResponseRouter()
        {
        }

        public static NoopResponseRouter Instance { get; } = new NoopResponseRouter();

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

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => Task.FromResult<TResponse>(default!);

        bool IResponseRouter.TryGetRequest(long id, [NotNullWhen(true)] out string method, [NotNullWhen(true)] out TaskCompletionSource<JToken> pendingTask)
        {
            method = default!;
            pendingTask = default!;
            return false;
        }

        private class Impl : IResponseRouterReturns
        {
            public Task<TResponse> Returning<TResponse>(CancellationToken cancellationToken) => Task.FromResult<TResponse>(default!);

            public Task ReturningVoid(CancellationToken cancellationToken) => Task.CompletedTask;
        }
    }
}
