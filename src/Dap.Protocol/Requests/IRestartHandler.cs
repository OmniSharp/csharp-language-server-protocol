using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Restart, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IRestartHandler : IJsonRpcRequestHandler<RestartArguments, RestartResponse>
    {
    }

    public abstract class RestartHandler : IRestartHandler
    {
        public abstract Task<RestartResponse> Handle(RestartArguments request, CancellationToken cancellationToken);
    }
}
