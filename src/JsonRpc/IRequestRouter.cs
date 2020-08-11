using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestRouter
    {
    }

    public interface IRequestRouter<TDescriptor> : IRequestRouter
    {
        IRequestDescriptor<TDescriptor> GetDescriptors(Notification notification);
        IRequestDescriptor<TDescriptor> GetDescriptors(Request request);
        Task RouteNotification(IRequestDescriptor<TDescriptor> descriptors, Notification notification, CancellationToken token);
        Task<ErrorResponse> RouteRequest(IRequestDescriptor<TDescriptor> descriptors, Request request, CancellationToken token);
    }
}
