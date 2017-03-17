using System.Threading.Tasks;
using JsonRPC.Server;

namespace JsonRPC
{
    public interface IServer
    {
        void HandleNotification(Notification notification);
        Task<ErrorResponse> HandleRequest(Request request);
    }
}