using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Scopes, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IScopesHandler : IJsonRpcRequestHandler<ScopesArguments, ScopesResponse>
    {
    }

    public abstract class ScopesHandler : IScopesHandler
    {
        public abstract Task<ScopesResponse> Handle(ScopesArguments request, CancellationToken cancellationToken);
    }
}
