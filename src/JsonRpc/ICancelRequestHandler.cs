using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;



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

    public static class RequestHandler {

        public static DelegatingHandlers.Request<TParams, TResult> For<TParams, TResult>(
            Func<TParams, CancellationToken, Task<TResult>> handler)
            where TParams : IRequest<TResult>
        {
            return new DelegatingHandlers.Request<TParams, TResult>(handler);
        }

        public static DelegatingHandlers.Request<TParams, TResult> For<TParams, TResult>(
            Func<TParams, Task<TResult>> handler)
            where TParams : IRequest<TResult>
        {
            return new DelegatingHandlers.Request<TParams, TResult>(handler);
        }

        public static DelegatingHandlers.Request<TParams> For<TParams>(Func<TParams, CancellationToken, Task> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Request<TParams>(handler);
        }

        public static DelegatingHandlers.Request<TParams> For<TParams>(Func<TParams, Task> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Request<TParams>(handler);
        }
    }

    public static class NotificationHandler {

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Action<TParams, CancellationToken> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Notification<TParams>( handler);
        }

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Func<TParams, CancellationToken, Task> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Notification<TParams>(handler);
        }
        public static DelegatingHandlers.Notification<TParams> For<TParams>(Action<TParams> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Notification<TParams>((p, ct) => handler(p));
        }

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Func<TParams, Task> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Notification<TParams>((p, ct) => handler(p));
        }
    }
}
