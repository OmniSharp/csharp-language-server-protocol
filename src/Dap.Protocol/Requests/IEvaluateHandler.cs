using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Evaluate, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IEvaluateHandler : IJsonRpcRequestHandler<EvaluateArguments, EvaluateResponse>
    {
    }

    public abstract class EvaluateHandler : IEvaluateHandler
    {
        public abstract Task<EvaluateResponse> Handle(EvaluateArguments request, CancellationToken cancellationToken);
    }
}
