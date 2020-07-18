using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Process, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IProcessHandler : IJsonRpcNotificationHandler<ProcessEvent> { }

    public abstract class ProcessHandler : IProcessHandler
    {
        public abstract Task<Unit> Handle(ProcessEvent request, CancellationToken cancellationToken);
    }
}
