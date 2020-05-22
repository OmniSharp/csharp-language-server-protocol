using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Stopped, Direction.ServerToClient)]
    public interface IStoppedHandler : IJsonRpcNotificationHandler<StoppedEvent> { }

    public abstract class StoppedHandler : IStoppedHandler
    {
        public abstract Task<Unit> Handle(StoppedEvent request, CancellationToken cancellationToken);
    }

    public static class StoppedExtensions
    {
        public static IDisposable OnStopped(this IDebugAdapterClientRegistry registry, Action<StoppedEvent> handler)
        {
            return registry.AddHandler(EventNames.Stopped, NotificationHandler.For(handler));
        }

        public static IDisposable OnStopped(this IDebugAdapterClientRegistry registry, Action<StoppedEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Stopped, NotificationHandler.For(handler));
        }

        public static IDisposable OnStopped(this IDebugAdapterClientRegistry registry, Func<StoppedEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Stopped, NotificationHandler.For(handler));
        }

        public static IDisposable OnStopped(this IDebugAdapterClientRegistry registry, Func<StoppedEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Stopped, NotificationHandler.For(handler));
        }

        public static void SendStopped(this IDebugAdapterServer mediator, StoppedEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
