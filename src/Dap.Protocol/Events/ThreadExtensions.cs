using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Thread, Direction.ServerToClient)]
    public interface IThreadHandler : IJsonRpcNotificationHandler<ThreadEvent> { }

    public abstract class ThreadHandler : IThreadHandler
    {
        public abstract Task<Unit> Handle(ThreadEvent request, CancellationToken cancellationToken);
    }

    public static class ThreadExtensions
    {
        public static IDisposable OnThread(this IDebugAdapterClientRegistry registry, Action<ThreadEvent> handler)
        {
            return registry.AddHandler(EventNames.Thread, NotificationHandler.For(handler));
        }

        public static IDisposable OnThread(this IDebugAdapterClientRegistry registry, Action<ThreadEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Thread, NotificationHandler.For(handler));
        }

        public static IDisposable OnThread(this IDebugAdapterClientRegistry registry, Func<ThreadEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Thread, NotificationHandler.For(handler));
        }

        public static IDisposable OnThread(this IDebugAdapterClientRegistry registry, Func<ThreadEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Thread, NotificationHandler.For(handler));
        }

        public static void SendThread(this IDebugAdapterServer mediator, ThreadEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
