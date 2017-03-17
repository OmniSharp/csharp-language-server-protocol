using System.Threading.Tasks;
using JsonRPC.Server;

namespace JsonRPC
{
    public interface IMediator : IClient, IServer
    {
    }

    public interface IClient
    {
        Task SendNotification<T>(string method, T @params);
        Task<TResponse> SendRequest<T, TResponse>(string method, T @params);
        Task SendRequest<T>(string method, T @params);
    }

    public interface IServer
    {
        void HandleNotification(Notification notification);
        Task<ErrorResponse> HandleRequest(Request request);
    }
}