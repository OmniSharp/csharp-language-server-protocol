using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Thread, Direction.ServerToClient)]
    public class ThreadEvent : IRequest
    {
        /// <summary>
        /// The reason for the event.
        /// Values: 'started', 'exited', etc.
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// The identifier of the thread.
        /// </summary>
        public long ThreadId { get; set; }
    }
}
