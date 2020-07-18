using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepIn, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IStepInHandler : IJsonRpcRequestHandler<StepInArguments, StepInResponse>
    {
    }

    public abstract class StepInHandler : IStepInHandler
    {
        public abstract Task<StepInResponse> Handle(StepInArguments request, CancellationToken cancellationToken);
    }
}
