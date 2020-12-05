using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class HandlerAdapter
    {
        public static Func<TParams, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, Task<TResult>> handler) => (a, _) => handler(a);
        public static Func<TParams, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, CancellationToken, Task<TResult>> handler) => handler;

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Func<TParams, Task> handler) => (a, _) => handler(a);
        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Func<TParams, CancellationToken, Task> handler) => handler;

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Action<TParams> handler) => (a, _) => {
            handler(a);
            return Task.CompletedTask;
        };

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Action<TParams, CancellationToken> handler) => (a, t) => {
            handler(a, t);
            return Task.CompletedTask;
        };
    }
}
