using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        Task RouteNotification(Notification notification, CancellationToken token);
        Task<ErrorResponse> RouteRequest(Request request, CancellationToken token);
        void CancelRequest(object id);
        void StartRequest(object id);
    }

    public interface IRequestRouter<TDescriptor> : IRequestRouter
    {
        TDescriptor GetDescriptor(Notification notification);
        TDescriptor GetDescriptor(Request request);
        Task RouteNotification(TDescriptor descriptor, Notification notification, CancellationToken token);
        Task<ErrorResponse> RouteRequest(TDescriptor descriptor, Request request, CancellationToken token);
    }
}
