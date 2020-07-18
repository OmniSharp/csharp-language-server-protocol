using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Continue, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IContinueHandler : IJsonRpcRequestHandler<ContinueArguments, ContinueResponse>
    {
    }

    public abstract class ContinueHandler : IContinueHandler
    {
        public abstract Task<ContinueResponse> Handle(ContinueArguments request, CancellationToken cancellationToken);
    }
}
