using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.ProgressStart, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IProgressStartHandler : IJsonRpcNotificationHandler<ProgressStartEvent>
    {
    }

    public abstract class ProgressStartHandlerBase : IProgressStartHandler
    {
        public abstract Task<Unit> Handle(ProgressStartEvent request, CancellationToken cancellationToken);
    }
}
