using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        IHandlerDescriptor GetDescriptor(Notification notification);
        IHandlerDescriptor GetDescriptor(Request request);

        Task RouteNotification(Notification notification);
        Task RouteNotification(IHandlerDescriptor descriptor, Notification notification);
        Task<ErrorResponse> RouteRequest(Request request);
        Task<ErrorResponse> RouteRequest(IHandlerDescriptor descriptor, Request request);
    }
}
