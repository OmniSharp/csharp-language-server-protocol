using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReadMemory)]
    public interface IReadMemoryHandler : IJsonRpcRequestHandler<ReadMemoryArguments, ReadMemoryResponse> { }

    public abstract class ReadMemoryHandler : IReadMemoryHandler
    {
        public abstract Task<ReadMemoryResponse> Handle(ReadMemoryArguments request, CancellationToken cancellationToken);
    }

    public static class ReadMemoryHandlerExtensions
    {
        public static IDisposable OnReadMemory(this IDebugAdapterRegistry registry, Func<ReadMemoryArguments, CancellationToken, Task<ReadMemoryResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ReadMemoryHandler
        {
            private readonly Func<ReadMemoryArguments, CancellationToken, Task<ReadMemoryResponse>> _handler;

            public DelegatingHandler(Func<ReadMemoryArguments, CancellationToken, Task<ReadMemoryResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ReadMemoryResponse> Handle(ReadMemoryArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
