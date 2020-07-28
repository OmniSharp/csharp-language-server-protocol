using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetInstructionBreakpoints, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ISetInstructionBreakpointsHandler : IJsonRpcRequestHandler<SetInstructionBreakpointsArguments, SetInstructionBreakpointsResponse>
    {
    }

    public abstract class SetInstructionBreakpointsHandlerBase : ISetInstructionBreakpointsHandler
    {
        public abstract Task<SetInstructionBreakpointsResponse> Handle(SetInstructionBreakpointsArguments request, CancellationToken cancellationToken);
    }
}
