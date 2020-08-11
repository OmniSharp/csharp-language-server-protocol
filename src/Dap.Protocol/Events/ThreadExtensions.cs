using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.Thread, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IThreadHandler : IJsonRpcNotificationHandler<ThreadEvent>
    {
    }

    public abstract class ThreadHandler : IThreadHandler
    {
        public abstract Task<Unit> Handle(ThreadEvent request, CancellationToken cancellationToken);
    }
}
