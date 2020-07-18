using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{

    [Parallel, Method(EventNames.Breakpoint, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IBreakpointHandler : IJsonRpcNotificationHandler<BreakpointEvent> { }

    public abstract class BreakpointHandler : IBreakpointHandler
    {
        public abstract Task<Unit> Handle(BreakpointEvent request, CancellationToken cancellationToken);
    }
}
