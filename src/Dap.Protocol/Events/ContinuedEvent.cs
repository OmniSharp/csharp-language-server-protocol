using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Continued, Direction.ServerToClient)]
    public class ContinuedEvent : IRequest
    {
        /// <summary>
        /// The thread which was continued.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// If 'allThreadsContinued' is true, a debug adapter can announce that all threads have continued.
        /// </summary>
        [Optional]
        public bool AllThreadsContinued { get; set; }
    }
}
