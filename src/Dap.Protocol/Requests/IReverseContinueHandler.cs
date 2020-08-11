using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.ReverseContinue, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IReverseContinueHandler : IJsonRpcRequestHandler<ReverseContinueArguments, ReverseContinueResponse>
    {
    }

    public abstract class ReverseContinueHandler : IReverseContinueHandler
    {
        public abstract Task<ReverseContinueResponse> Handle(
            ReverseContinueArguments request,
            CancellationToken cancellationToken
        );
    }
}
