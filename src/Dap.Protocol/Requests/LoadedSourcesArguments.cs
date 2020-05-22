using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.LoadedSources, Direction.ClientToServer)]
    public class LoadedSourcesArguments : IRequest<LoadedSourcesResponse>
    {
    }

}
