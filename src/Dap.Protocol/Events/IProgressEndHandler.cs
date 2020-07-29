using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.ProgressEnd, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IProgressEndHandler : IJsonRpcNotificationHandler<ProgressEndEvent>
    {
    }

    public abstract class ProgressEndHandlerBase : IProgressEndHandler
    {
        public abstract Task<Unit> Handle(ProgressEndEvent request, CancellationToken cancellationToken);
    }
}
