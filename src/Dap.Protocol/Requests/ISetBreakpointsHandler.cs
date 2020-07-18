using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetBreakpoints, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ISetBreakpointsHandler : IJsonRpcRequestHandler<SetBreakpointsArguments, SetBreakpointsResponse>
    {
    }

    public abstract class SetBreakpointsHandler : ISetBreakpointsHandler
    {
        public abstract Task<SetBreakpointsResponse> Handle(SetBreakpointsArguments request,
            CancellationToken cancellationToken);
    }
}
