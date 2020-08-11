using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Disconnect, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IDisconnectHandler : IJsonRpcRequestHandler<DisconnectArguments, DisconnectResponse>
    {
    }

    public abstract class DisconnectHandler : IDisconnectHandler
    {
        public abstract Task<DisconnectResponse> Handle(
            DisconnectArguments request,
            CancellationToken cancellationToken
        );
    }
}
