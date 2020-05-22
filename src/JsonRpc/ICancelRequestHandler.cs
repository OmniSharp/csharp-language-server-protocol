using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;


namespace OmniSharp.Extensions.JsonRpc
{
    [Parallel, Method(JsonRpcNames.CancelRequest, Direction.Bidirectional)]
    public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

    public abstract class CancelRequestHandler : ICancelRequestHandler
    {
        public abstract Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken);
    }

    public static class CancelRequestExtensions
    {
        public static IDisposable OnCancelRequest(
            this IJsonRpcHandlerRegistry registry,
            Action<CancelParams> handler)
        {
            return registry.AddHandler(JsonRpcNames.CancelRequest, NotificationHandler.For(handler));
        }

        public static IDisposable OnCancelRequest(
            this IJsonRpcHandlerRegistry registry,
            Func<CancelParams, Task> handler)
        {
            return registry.AddHandler(JsonRpcNames.CancelRequest, NotificationHandler.For(handler));
        }
    }
}
