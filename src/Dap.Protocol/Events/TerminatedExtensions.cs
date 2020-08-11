using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.Terminated, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ITerminatedHandler : IJsonRpcNotificationHandler<TerminatedEvent>
    {
    }

    public abstract class TerminatedHandler : ITerminatedHandler
    {
        public abstract Task<Unit> Handle(TerminatedEvent request, CancellationToken cancellationToken);
    }
}
