using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReadMemory, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IReadMemoryHandler : IJsonRpcRequestHandler<ReadMemoryArguments, ReadMemoryResponse>
    {
    }

    public abstract class ReadMemoryHandler : IReadMemoryHandler
    {
        public abstract Task<ReadMemoryResponse> Handle(ReadMemoryArguments request,
            CancellationToken cancellationToken);
    }
}
