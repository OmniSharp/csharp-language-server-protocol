using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.StepInTargets, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IStepInTargetsHandler : IJsonRpcRequestHandler<StepInTargetsArguments, StepInTargetsResponse>
    {
    }

    public abstract class StepInTargetsHandler : IStepInTargetsHandler
    {
        public abstract Task<StepInTargetsResponse> Handle(
            StepInTargetsArguments request,
            CancellationToken cancellationToken
        );
    }
}
