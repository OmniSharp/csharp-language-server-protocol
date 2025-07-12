using OmniSharp.Extensions.JsonRpc.Server;
using Notification = OmniSharp.Extensions.JsonRpc.Server.Notification;

namespace OmniSharp.Extensions.JsonRpc
{
    public abstract class RequestInvoker : IDisposable
    {
        public abstract RequestInvocationHandle InvokeRequest(IRequestDescriptor<IHandlerDescriptor?> descriptor, Request request);

        public abstract void InvokeNotification(IRequestDescriptor<IHandlerDescriptor?> descriptor, Notification notification);

        public abstract void Dispose();
    }
}
