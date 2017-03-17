using System.Threading.Tasks;
using JsonRpc.Server;

namespace JsonRpc
{
    public interface IServer
    {
        void HandleNotification(Notification notification);
        Task<ErrorResponse> HandleRequest(Request request);
        void CancelRequest(object id);
    }
}