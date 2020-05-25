using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Threads, Direction.ClientToServer)]
    public interface IThreadsHandler : IJsonRpcRequestHandler<ThreadsArguments, ThreadsResponse>
    {
    }

    public abstract class ThreadsHandler : IThreadsHandler
    {
        public abstract Task<ThreadsResponse> Handle(ThreadsArguments request, CancellationToken cancellationToken);
    }

    public static class ThreadsExtensions
    {
        public static IDebugAdapterServerRegistry OnThreads(this IDebugAdapterServerRegistry registry,
            Func<ThreadsArguments, CancellationToken, Task<ThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Threads, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnThreads(this IDebugAdapterServerRegistry registry,
            Func<ThreadsArguments, Task<ThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Threads, RequestHandler.For(handler));
        }

        public static Task<ThreadsResponse> RequestThreads(this IDebugAdapterClient mediator, ThreadsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
