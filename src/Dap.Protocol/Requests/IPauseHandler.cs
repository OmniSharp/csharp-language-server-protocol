using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Pause, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IPauseHandler : IJsonRpcRequestHandler<PauseArguments, PauseResponse>
    {
    }

    public abstract class PauseHandler : IPauseHandler
    {
        public abstract Task<PauseResponse> Handle(PauseArguments request, CancellationToken cancellationToken);
    }
}
