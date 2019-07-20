using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ContinuedEvent : IRequest
    {


        /// <summary>
        /// The thread which was continued.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// If 'allThreadsContinued' is true, a debug adapter can announce that all threads have continued.
        /// </summary>
        [Optional] public bool? AllThreadsContinued { get; set; }
    }

}
