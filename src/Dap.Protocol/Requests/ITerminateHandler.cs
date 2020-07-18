using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Terminate, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ITerminateHandler : IJsonRpcRequestHandler<TerminateArguments, TerminateResponse>
    {
    }

    public abstract class TerminateHandler : ITerminateHandler
    {
        public abstract Task<TerminateResponse> Handle(TerminateArguments request, CancellationToken cancellationToken);
    }
}
