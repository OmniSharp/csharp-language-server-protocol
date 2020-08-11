using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.SetDataBreakpoints, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface
        ISetDataBreakpointsHandler : IJsonRpcRequestHandler<SetDataBreakpointsArguments, SetDataBreakpointsResponse>
    {
    }

    public abstract class SetDataBreakpointsHandler : ISetDataBreakpointsHandler
    {
        public abstract Task<SetDataBreakpointsResponse> Handle(
            SetDataBreakpointsArguments request,
            CancellationToken cancellationToken
        );
    }
}
