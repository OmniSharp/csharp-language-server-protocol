using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Initialized, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedEvent> { }

    public abstract class InitializedHandler : IInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedEvent request, CancellationToken cancellationToken);
    }
}
