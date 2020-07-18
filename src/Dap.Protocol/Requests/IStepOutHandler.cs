using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepOut, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IStepOutHandler : IJsonRpcRequestHandler<StepOutArguments, StepOutResponse>
    {
    }

    public abstract class StepOutHandler : IStepOutHandler
    {
        public abstract Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken);
    }
}
