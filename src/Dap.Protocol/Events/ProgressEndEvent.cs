using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.ProgressEnd, Direction.ServerToClient)]
    public class ProgressEndEvent : ProgressEvent, IRequest
    {
    }
}
