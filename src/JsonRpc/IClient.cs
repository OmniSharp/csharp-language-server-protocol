using System.Threading;
using System.Threading.Tasks;

namespace JsonRpc
{
    public interface IClient
    {
        Task SendNotification<T>(string method, T @params);
        Task<TResponse> SendRequest<T, TResponse>(string method, T @params);
        Task SendRequest<T>(string method, T @params);
    }
}