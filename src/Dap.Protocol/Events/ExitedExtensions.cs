using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.Exited, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IExitedHandler : IJsonRpcNotificationHandler<ExitedEvent>
    {
    }

    public abstract class ExitedHandler : IExitedHandler
    {
        public abstract Task<Unit> Handle(ExitedEvent request, CancellationToken cancellationToken);
    }
}
