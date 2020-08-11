using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.TerminateThreads, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ITerminateThreadsHandler : IJsonRpcRequestHandler<TerminateThreadsArguments, TerminateThreadsResponse>
    {
    }

    public abstract class TerminateThreadsHandler : ITerminateThreadsHandler
    {
        public abstract Task<TerminateThreadsResponse> Handle(
            TerminateThreadsArguments request,
            CancellationToken cancellationToken
        );
    }
}
