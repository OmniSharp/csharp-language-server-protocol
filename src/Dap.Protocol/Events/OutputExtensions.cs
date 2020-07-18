using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Output, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IOutputHandler : IJsonRpcNotificationHandler<OutputEvent> { }

    public abstract class OutputHandler : IOutputHandler
    {
        public abstract Task<Unit> Handle(OutputEvent request, CancellationToken cancellationToken);
    }
}
