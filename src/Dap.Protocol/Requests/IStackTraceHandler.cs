using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StackTrace, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IStackTraceHandler : IJsonRpcRequestHandler<StackTraceArguments, StackTraceResponse>
    {
    }

    public abstract class StackTraceHandler : IStackTraceHandler
    {
        public abstract Task<StackTraceResponse> Handle(StackTraceArguments request,
            CancellationToken cancellationToken);
    }
}
