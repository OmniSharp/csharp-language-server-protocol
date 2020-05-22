using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
        IServiceProvider ServiceProvider { get; }
        void CancelRequest(object id);
        void StartRequest(object id, IHandlerDescriptor descriptor);
        IHandlerDescriptor GetRequestDescriptor(object id);
    }

    public interface IRequestRouter<TDescriptor> : IRequestRouter
    {
        TDescriptor GetDescriptor(Notification notification);
        TDescriptor GetDescriptor(Request request);
        Task RouteNotification(TDescriptor descriptor, Notification notification, CancellationToken token, CancellationToken contentModifiedToken);
        Task<ErrorResponse> RouteRequest(TDescriptor descriptor, Request request, CancellationToken token, CancellationToken contentModifiedToken);
    }
}
