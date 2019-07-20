using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Threads)]
    public interface IThreadsHandler : IJsonRpcRequestHandler<ThreadsArguments, ThreadsResponse> { }

    public abstract class ThreadsHandler : IThreadsHandler
    {
        public abstract Task<ThreadsResponse> Handle(ThreadsArguments request, CancellationToken cancellationToken);
    }

    public static class ThreadsHandlerExtensions
    {
        public static IDisposable OnThreads(this IDebugAdapterRegistry registry, Func<ThreadsArguments, CancellationToken, Task<ThreadsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ThreadsHandler
        {
            private readonly Func<ThreadsArguments, CancellationToken, Task<ThreadsResponse>> _handler;

            public DelegatingHandler(Func<ThreadsArguments, CancellationToken, Task<ThreadsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ThreadsResponse> Handle(ThreadsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
