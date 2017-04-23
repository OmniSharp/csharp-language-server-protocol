using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public interface IResponseRouter
    {
        Task SendNotification<T>(string method, T @params);
        Task<TResponse> SendRequest<T, TResponse>(string method, T @params);
        Task SendRequest<T>(string method, T @params);
        TaskCompletionSource<JToken> GetRequest(long id);
    }
}