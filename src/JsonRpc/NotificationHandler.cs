using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class NotificationHandler
    {
        public static DelegatingHandlers.Notification<TParams> For<TParams>(Action<TParams, CancellationToken> handler)
            where TParams : IRequest<Unit> => new(HandlerAdapter.Adapt(handler));

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Func<TParams, CancellationToken, Task> handler)
            where TParams : IRequest<Unit> => new(HandlerAdapter.Adapt(handler));

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Action<TParams> handler)
            where TParams : IRequest<Unit> => new(HandlerAdapter.Adapt(handler));

        public static DelegatingHandlers.Notification<TParams> For<TParams>(Func<TParams, Task> handler)
            where TParams : IRequest<Unit> => new(HandlerAdapter.Adapt(handler));
    }
}
