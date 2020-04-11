using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IResponseRouter
    {
        void SendNotification(string method);
        void SendNotification<T>(string method, T @params);
        void SendNotification(IRequest @params);
        Task<TResponse> SendRequest<T, TResponse>(string method, T @params, CancellationToken cancellationToken);
        Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken);
        Task SendRequest(IRequest @params, CancellationToken cancellationToken);
        Task<TResponse> SendRequest<TResponse>(string method, CancellationToken cancellationToken);
        Task SendRequest<T>(string method, T @params, CancellationToken cancellationToken);
        TaskCompletionSource<JToken> GetRequest(long id);
    }
}
