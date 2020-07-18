using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Module, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IModuleHandler : IJsonRpcNotificationHandler<ModuleEvent> { }

    public abstract class ModuleHandler : IModuleHandler
    {
        public abstract Task<Unit> Handle(ModuleEvent request, CancellationToken cancellationToken);
    }
}
