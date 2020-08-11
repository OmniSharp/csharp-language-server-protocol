using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.BreakpointLocations, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IBreakpointLocationsHandler : IJsonRpcRequestHandler<BreakpointLocationsArguments, BreakpointLocationsResponse>
    {
    }

    public abstract class BreakpointLocationsHandlerBase : IBreakpointLocationsHandler
    {
        public abstract Task<BreakpointLocationsResponse> Handle(BreakpointLocationsArguments request, CancellationToken cancellationToken);
    }
}
