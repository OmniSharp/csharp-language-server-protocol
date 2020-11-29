using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class RequestHandler
    {
        public static DelegatingHandlers.Request<TParams, TResult> For<TParams, TResult>(Func<TParams, CancellationToken, Task<TResult>> handler)
            where TParams : IRequest<TResult> => new(handler);

        public static DelegatingHandlers.Request<TParams, TResult> For<TParams, TResult>(Func<TParams, Task<TResult>> handler)
            where TParams : IRequest<TResult> => new(handler);

        public static DelegatingHandlers.Request<TParams> For<TParams>(Func<TParams, CancellationToken, Task> handler)
            where TParams : IRequest => new(handler);

        public static DelegatingHandlers.Request<TParams> For<TParams>(Func<TParams, Task> handler)
            where TParams : IRequest => new(handler);
    }
}
