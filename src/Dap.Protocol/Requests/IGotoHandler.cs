using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Goto, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IGotoHandler : IJsonRpcRequestHandler<GotoArguments, GotoResponse>
    {
    }

    public abstract class GotoHandler : IGotoHandler
    {
        public abstract Task<GotoResponse> Handle(GotoArguments request, CancellationToken cancellationToken);
    }
}
