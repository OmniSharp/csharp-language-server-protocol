using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Initialized, Direction.ServerToClient)]
    public class InitializedEvent : IRequest
    {
    }
}
