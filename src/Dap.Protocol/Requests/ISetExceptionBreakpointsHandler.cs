using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.SetExceptionBreakpoints, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ISetExceptionBreakpointsHandler : IJsonRpcRequestHandler<SetExceptionBreakpointsArguments,
        SetExceptionBreakpointsResponse>
    {
    }

    public abstract class SetExceptionBreakpointsHandler : ISetExceptionBreakpointsHandler
    {
        public abstract Task<SetExceptionBreakpointsResponse> Handle(
            SetExceptionBreakpointsArguments request,
            CancellationToken cancellationToken
        );
    }
}
