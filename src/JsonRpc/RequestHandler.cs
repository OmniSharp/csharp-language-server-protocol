using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;



namespace OmniSharp.Extensions.JsonRpc
{
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
}
