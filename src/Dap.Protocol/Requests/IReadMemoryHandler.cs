using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReadMemory, Direction.ClientToServer)]
    public interface IReadMemoryHandler : IJsonRpcRequestHandler<ReadMemoryArguments, ReadMemoryResponse>
    {
    }

    public abstract class ReadMemoryHandler : IReadMemoryHandler
    {
        public abstract Task<ReadMemoryResponse> Handle(ReadMemoryArguments request,
            CancellationToken cancellationToken);
    }

    public static class ReadMemoryExtensions
    {
        public static IDebugAdapterServerRegistry OnReadMemory(this IDebugAdapterServerRegistry registry,
            Func<ReadMemoryArguments, CancellationToken, Task<ReadMemoryResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReadMemory, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnReadMemory(this IDebugAdapterServerRegistry registry,
            Func<ReadMemoryArguments, Task<ReadMemoryResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReadMemory, RequestHandler.For(handler));
        }

        public static Task<ReadMemoryResponse> RequestReadMemory(this IDebugAdapterClient mediator, ReadMemoryArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
