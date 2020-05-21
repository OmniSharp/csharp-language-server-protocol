using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Terminated, Direction.ServerToClient)]
    public interface ITerminatedHandler : IJsonRpcNotificationHandler<TerminatedEvent> { }

    public abstract class TerminatedHandler : ITerminatedHandler
    {
        public abstract Task<Unit> Handle(TerminatedEvent request, CancellationToken cancellationToken);
    }

    public static class TerminatedExtensions
    {
        public static IDisposable OnTerminated(this IDebugAdapterClientRegistry registry, Action<TerminatedEvent> handler)
        {
            return registry.AddHandler(EventNames.Terminated, NotificationHandler.For(handler));
        }

        public static IDisposable OnTerminated(this IDebugAdapterClientRegistry registry, Action<TerminatedEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Terminated, NotificationHandler.For(handler));
        }

        public static IDisposable OnTerminated(this IDebugAdapterClientRegistry registry, Func<TerminatedEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Terminated, NotificationHandler.For(handler));
        }

        public static IDisposable OnTerminated(this IDebugAdapterClientRegistry registry, Func<TerminatedEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Terminated, NotificationHandler.For(handler));
        }

        public static void SendTerminated(this IDebugAdapterServer mediator, TerminatedEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
