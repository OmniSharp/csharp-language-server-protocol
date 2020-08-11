using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.ConfigurationDone, Direction.ClientToServer)]
    public class ConfigurationDoneArguments : IRequest<ConfigurationDoneResponse>
    {
    }
}
