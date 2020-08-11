using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.SetFunctionBreakpoints, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface
        ISetFunctionBreakpointsHandler : IJsonRpcRequestHandler<SetFunctionBreakpointsArguments,
            SetFunctionBreakpointsResponse>
    {
    }

    public abstract class SetFunctionBreakpointsHandler : ISetFunctionBreakpointsHandler
    {
        public abstract Task<SetFunctionBreakpointsResponse> Handle(
            SetFunctionBreakpointsArguments request,
            CancellationToken cancellationToken
        );
    }
}
