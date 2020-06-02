using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Capabilities, Direction.ServerToClient)]
    public interface ICapabilitiesHandler : IJsonRpcNotificationHandler<CapabilitiesEvent> { }

    public abstract class CapabilitiesHandler : ICapabilitiesHandler
    {
        public abstract Task<Unit> Handle(CapabilitiesEvent request, CancellationToken cancellationToken);
    }

    public static class CapabilitiesExtensions
    {
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent> handler)
        {
            return registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        }

        public static void SendCapabilities(this IDebugAdapterServer mediator, CapabilitiesEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
