using System.Threading.Tasks;
using JsonRpc.Server;

namespace JsonRpc
{
    public interface IIncomingRequestRouter
    {
        void RouteNotification(Notification notification);
        Task<ErrorResponse> RouteRequest(Request request);
        // void CancelRequest(object id);
    }
}