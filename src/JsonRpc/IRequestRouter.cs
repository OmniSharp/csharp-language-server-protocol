using System;
using System.Threading.Tasks;
using JsonRpc.Server;

namespace JsonRpc
{
    public interface IRequestRouter
    {
        void RouteNotification(Notification notification);
        Task<ErrorResponse> RouteRequest(Request request);

        IDisposable Add(IJsonRpcHandler handler);
        void Remove(IJsonRpcHandler handler);
    }
}