using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.Stopped, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IStoppedHandler : IJsonRpcNotificationHandler<StoppedEvent>
    {
    }

    public abstract class StoppedHandler : IStoppedHandler
    {
        public abstract Task<Unit> Handle(StoppedEvent request, CancellationToken cancellationToken);
    }
}
