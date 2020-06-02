using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class NotificationHandler
    {
        public static DelegatingHandlers.Notification<TParams> For<TParams>(Action<TParams, CancellationToken> handler)
            where TParams : IRequest
        {
            return new DelegatingHandlers.Notification<TParams>(handler);
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
