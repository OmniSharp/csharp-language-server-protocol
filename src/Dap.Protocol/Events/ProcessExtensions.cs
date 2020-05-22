using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Process, Direction.ServerToClient)]
    public interface IProcessHandler : IJsonRpcNotificationHandler<ProcessEvent> { }

    public abstract class ProcessHandler : IProcessHandler
    {
        public abstract Task<Unit> Handle(ProcessEvent request, CancellationToken cancellationToken);
    }

    public static class ProcessExtensions
    {
        public static IDisposable OnProcess(this IDebugAdapterClientRegistry registry, Action<ProcessEvent> handler)
        {
            return registry.AddHandler(EventNames.Process, NotificationHandler.For(handler));
        }

        public static IDisposable OnProcess(this IDebugAdapterClientRegistry registry, Action<ProcessEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Process, NotificationHandler.For(handler));
        }

        public static IDisposable OnProcess(this IDebugAdapterClientRegistry registry, Func<ProcessEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Process, NotificationHandler.For(handler));
        }

        public static IDisposable OnProcess(this IDebugAdapterClientRegistry registry, Func<ProcessEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Process, NotificationHandler.For(handler));
        }

        public static void SendProcess(this IDebugAdapterServer mediator, ProcessEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
