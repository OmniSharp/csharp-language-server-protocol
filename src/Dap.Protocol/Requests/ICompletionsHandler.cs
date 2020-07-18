using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Completions, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ICompletionsHandler : IJsonRpcRequestHandler<CompletionsArguments, CompletionsResponse> { }

    public abstract class CompletionsHandler : ICompletionsHandler
    {
        public abstract Task<CompletionsResponse> Handle(CompletionsArguments request, CancellationToken cancellationToken);
    }
}
