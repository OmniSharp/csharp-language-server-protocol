using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.LoadedSources, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ILoadedSourcesHandler : IJsonRpcRequestHandler<LoadedSourcesArguments, LoadedSourcesResponse>
    {
    }

    public abstract class LoadedSourcesHandler : ILoadedSourcesHandler
    {
        public abstract Task<LoadedSourcesResponse> Handle(LoadedSourcesArguments request,
            CancellationToken cancellationToken);
    }
}
