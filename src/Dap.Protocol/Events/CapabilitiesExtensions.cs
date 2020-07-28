using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Capabilities, Direction.ServerToClient)]
    [GenerateRequestMethods, GenerateHandlerMethods]
    public interface ICapabilitiesHandler : IJsonRpcNotificationHandler<CapabilitiesEvent>
    {
    }

    public abstract class CapabilitiesHandler : ICapabilitiesHandler
    {
        public abstract Task<Unit> Handle(CapabilitiesEvent request, CancellationToken cancellationToken);
    }
}
