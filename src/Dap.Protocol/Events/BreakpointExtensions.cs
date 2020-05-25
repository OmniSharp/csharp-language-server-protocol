using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Breakpoint, Direction.ServerToClient)]
    public interface IBreakpointHandler : IJsonRpcNotificationHandler<BreakpointEvent> { }

    public abstract class BreakpointHandler : IBreakpointHandler
    {
        public abstract Task<Unit> Handle(BreakpointEvent request, CancellationToken cancellationToken);
    }

    public static class BreakpointExtensions
    {
        public static IDebugAdapterClientRegistry OnBreakpoint(this IDebugAdapterClientRegistry registry, Action<BreakpointEvent> handler)
        {
            return registry.AddHandler(EventNames.Breakpoint, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnBreakpoint(this IDebugAdapterClientRegistry registry, Action<BreakpointEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Breakpoint, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnBreakpoint(this IDebugAdapterClientRegistry registry, Func<BreakpointEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Breakpoint, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnBreakpoint(this IDebugAdapterClientRegistry registry, Func<BreakpointEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Breakpoint, NotificationHandler.For(handler));
        }

        public static void SendBreakpoint(this IDebugAdapterServer mediator, BreakpointEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
