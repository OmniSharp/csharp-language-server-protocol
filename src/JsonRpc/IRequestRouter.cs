using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        Task RouteNotification(Notification notification);
        Task<ErrorResponse> RouteRequest(Request request);
    }
}
