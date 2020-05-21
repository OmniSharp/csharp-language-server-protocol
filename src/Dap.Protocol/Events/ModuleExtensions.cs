using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Module, Direction.ServerToClient)]
    public interface IModuleHandler : IJsonRpcNotificationHandler<ModuleEvent> { }

    public abstract class ModuleHandler : IModuleHandler
    {
        public abstract Task<Unit> Handle(ModuleEvent request, CancellationToken cancellationToken);
    }

    public static class ModuleExtensions
    {
        public static IDisposable OnModule(this IDebugAdapterClientRegistry registry, Action<ModuleEvent> handler)
        {
            return registry.AddHandler(EventNames.Module, NotificationHandler.For(handler));
        }

        public static IDisposable OnModule(this IDebugAdapterClientRegistry registry, Action<ModuleEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Module, NotificationHandler.For(handler));
        }

        public static IDisposable OnModule(this IDebugAdapterClientRegistry registry, Func<ModuleEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Module, NotificationHandler.For(handler));
        }

        public static IDisposable OnModule(this IDebugAdapterClientRegistry registry, Func<ModuleEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Module, NotificationHandler.For(handler));
        }

        public static void SendModule(this IDebugAdapterServer mediator, ModuleEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
