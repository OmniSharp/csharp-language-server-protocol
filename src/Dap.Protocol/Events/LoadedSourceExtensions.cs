using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel]
    [Method(EventNames.LoadedSource, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ILoadedSourceHandler : IJsonRpcNotificationHandler<LoadedSourceEvent>
    {
    }

    public abstract class LoadedSourceHandler : ILoadedSourceHandler
    {
        public abstract Task<Unit> Handle(LoadedSourceEvent request, CancellationToken cancellationToken);
    }
}
