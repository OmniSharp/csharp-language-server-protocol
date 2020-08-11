using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.Continued, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IContinuedHandler : IJsonRpcNotificationHandler<ContinuedEvent>
    {
    }

    public abstract class ContinuedHandler : IContinuedHandler
    {
        public abstract Task<Unit> Handle(ContinuedEvent request, CancellationToken cancellationToken);
    }
}
