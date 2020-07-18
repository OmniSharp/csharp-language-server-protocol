using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Threads, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IThreadsHandler : IJsonRpcRequestHandler<ThreadsArguments, ThreadsResponse>
    {
    }

    public abstract class ThreadsHandler : IThreadsHandler
    {
        public abstract Task<ThreadsResponse> Handle(ThreadsArguments request, CancellationToken cancellationToken);
    }
}
