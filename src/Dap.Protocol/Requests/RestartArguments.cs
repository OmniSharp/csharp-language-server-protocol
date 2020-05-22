using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Restart, Direction.ClientToServer)]
    public class RestartArguments : IRequest<RestartResponse>
    {
    }

}
