using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Source, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ISourceHandler : IJsonRpcRequestHandler<SourceArguments, SourceResponse>
    {
    }

    public abstract class SourceHandler : ISourceHandler
    {
        public abstract Task<SourceResponse> Handle(SourceArguments request, CancellationToken cancellationToken);
    }
}
