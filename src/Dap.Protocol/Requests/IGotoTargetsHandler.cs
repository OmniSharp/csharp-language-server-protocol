using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.GotoTargets, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IGotoTargetsHandler : IJsonRpcRequestHandler<GotoTargetsArguments, GotoTargetsResponse>
    {
    }


    public abstract class GotoTargetsHandler : IGotoTargetsHandler
    {
        public abstract Task<GotoTargetsResponse> Handle(GotoTargetsArguments request,
            CancellationToken cancellationToken);
    }
}
