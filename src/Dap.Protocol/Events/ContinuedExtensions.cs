using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Continued, Direction.ServerToClient)]
    public interface IContinuedHandler : IJsonRpcNotificationHandler<ContinuedEvent> { }

    public abstract class ContinuedHandler : IContinuedHandler
    {
        public abstract Task<Unit> Handle(ContinuedEvent request, CancellationToken cancellationToken);
    }

    public static class ContinuedExtensions
    {
        public static IDisposable OnContinued(this IDebugAdapterClientRegistry registry, Action<ContinuedEvent> handler)
        {
            return registry.AddHandler(EventNames.Continued, NotificationHandler.For(handler));
        }

        public static IDisposable OnContinued(this IDebugAdapterClientRegistry registry, Action<ContinuedEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Continued, NotificationHandler.For(handler));
        }

        public static IDisposable OnContinued(this IDebugAdapterClientRegistry registry, Func<ContinuedEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Continued, NotificationHandler.For(handler));
        }

        public static IDisposable OnContinued(this IDebugAdapterClientRegistry registry, Func<ContinuedEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Continued, NotificationHandler.For(handler));
        }

        public static void SendContinued(this IDebugAdapterServer mediator, ContinuedEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
