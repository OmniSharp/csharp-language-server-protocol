using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ThreadEvent : IRequest
    {

        /// <summary>
        /// The reason for the event.
        /// Values: 'started', 'exited', etc.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The identifier of the thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
