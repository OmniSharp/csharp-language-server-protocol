using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Output, Direction.ServerToClient)]
    public interface IOutputHandler : IJsonRpcNotificationHandler<OutputEvent> { }

    public abstract class OutputHandler : IOutputHandler
    {
        public abstract Task<Unit> Handle(OutputEvent request, CancellationToken cancellationToken);
    }

    public static class OutputExtensions
    {
        public static IDisposable OnOutput(this IDebugAdapterClientRegistry registry, Action<OutputEvent> handler)
        {
            return registry.AddHandler(EventNames.Output, NotificationHandler.For(handler));
        }

        public static IDisposable OnOutput(this IDebugAdapterClientRegistry registry, Action<OutputEvent, CancellationToken> handler)
        {
            return registry.AddHandler(EventNames.Output, NotificationHandler.For(handler));
        }

        public static IDisposable OnOutput(this IDebugAdapterClientRegistry registry, Func<OutputEvent, Task> handler)
        {
            return registry.AddHandler(EventNames.Output, NotificationHandler.For(handler));
        }

        public static IDisposable OnOutput(this IDebugAdapterClientRegistry registry, Func<OutputEvent, CancellationToken, Task> handler)
        {
            return registry.AddHandler(EventNames.Output, NotificationHandler.For(handler));
        }

        public static void SendOutput(this IDebugAdapterServer mediator, OutputEvent @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
