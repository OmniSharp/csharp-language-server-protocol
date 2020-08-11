using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Disassemble, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IDisassembleHandler : IJsonRpcRequestHandler<DisassembleArguments, DisassembleResponse>
    {
    }

    public abstract class DisassembleHandler : IDisassembleHandler
    {
        public abstract Task<DisassembleResponse> Handle(
            DisassembleArguments request,
            CancellationToken cancellationToken
        );
    }
}
