using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.LoadedSource, Direction.ServerToClient)]
    public interface ILoadedSourceHandler : IJsonRpcNotificationHandler<LoadedSourceEvent> { }

    public abstract class LoadedSourceHandler : ILoadedSourceHandler
    {
        public abstract Task<Unit> Handle(LoadedSourceEvent request, CancellationToken cancellationToken);
    }

    public static class LoadedSourceExtensions
    {
        public static IDisposable OnLoadedSource(this IDebugAdapterClientRegistry registry, Action<LoadedSourceEvent> handler)
        {
            return registry.AddHandler(EventNames.LoadedSource, NotificationHandler.For(handler));
        }

        public static IDisposable OnLoadedSource(this IDebugAdapterClientRegistry registry, Action<LoadedSourceEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.LoadedSource, NotificationHandler.For(handler));
        }

        public static IDisposable OnLoadedSource(this IDebugAdapterClientRegistry registry, Func<LoadedSourceEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.LoadedSource, NotificationHandler.For(handler));
        }

        public static IDisposable OnLoadedSource(this IDebugAdapterClientRegistry registry, Func<LoadedSourceEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.LoadedSource, NotificationHandler.For(handler));
        }

        public static void SendLoadedSource(this IDebugAdapterServer mediator, LoadedSourceEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
