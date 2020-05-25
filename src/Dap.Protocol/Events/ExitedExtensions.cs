using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Exited, Direction.ServerToClient)]
    public interface IExitedHandler : IJsonRpcNotificationHandler<ExitedEvent> { }

    public abstract class ExitedHandler : IExitedHandler
    {
        public abstract Task<Unit> Handle(ExitedEvent request, CancellationToken cancellationToken);
    }

    public static class ExitedExtensions
    {
        public static IDebugAdapterClientRegistry OnExited(this IDebugAdapterClientRegistry registry, Action<ExitedEvent> handler)
        {
            return registry.AddHandler(EventNames.Exited, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnExited(this IDebugAdapterClientRegistry registry, Action<ExitedEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Exited, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnExited(this IDebugAdapterClientRegistry registry, Func<ExitedEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Exited, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnExited(this IDebugAdapterClientRegistry registry, Func<ExitedEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Exited, NotificationHandler.For(handler));
        }

        public static void SendExited(this IDebugAdapterServer mediator, ExitedEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
