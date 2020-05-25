using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Initialized, Direction.ServerToClient)]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedEvent> { }

    public abstract class InitializedHandler : IInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedEvent request, CancellationToken cancellationToken);
    }

    public static class InitializedExtensions
    {
        public static IDebugAdapterClientRegistry OnInitialized(this IDebugAdapterClientRegistry registry, Action<InitializedEvent> handler)
        {
            return registry.AddHandler(EventNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnInitialized(this IDebugAdapterClientRegistry registry, Action<InitializedEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnInitialized(this IDebugAdapterClientRegistry registry, Func<InitializedEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Initialized, NotificationHandler.For(handler));
        }

        public static IDebugAdapterClientRegistry OnInitialized(this IDebugAdapterClientRegistry registry, Func<InitializedEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Initialized, NotificationHandler.For(handler));
        }

        public static void SendInitialized(this IDebugAdapterServer mediator, InitializedEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
