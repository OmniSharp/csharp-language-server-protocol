using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IResponseRouter
    {
        void SendNotification(string method);
        void SendNotification<T>(string method, T @params);
        Task<TResponse> SendRequest<T, TResponse>(string method, T @params, CancellationToken cancellationToken);
        Task<TResponse> SendRequest<TResponse>(string method, CancellationToken cancellationToken);
        Task SendRequest<T>(string method, T @params, CancellationToken cancellationToken);
        TaskCompletionSource<JToken> GetRequest(long id);
    }
}
