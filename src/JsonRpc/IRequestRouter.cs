using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        void RouteNotification(Notification notification);
        Task<ErrorResponse> RouteRequest(Request request);

        IDisposable Add(IJsonRpcHandler handler);
    }
}