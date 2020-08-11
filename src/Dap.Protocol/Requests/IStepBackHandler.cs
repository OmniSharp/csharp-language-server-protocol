using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.StepBack, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IStepBackHandler : IJsonRpcRequestHandler<StepBackArguments, StepBackResponse>
    {
    }

    public abstract class StepBackHandler : IStepBackHandler
    {
        public abstract Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken);
    }
}
