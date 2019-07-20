using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StackTraceArguments : IRequest<StackTraceResponse>
    {
        /// <summary>
        /// Retrieve the stacktrace for this thread.
        /// </summary>
        public long threadId { get; set; }

        /// <summary>
        /// The index of the first frame to return; if omitted frames start at 0.
        /// </summary>
        [Optional] public long? startFrame { get; set; }

        /// <summary>
        /// The maximum number of frames to return. If levels is not specified or 0, all frames are returned.
        /// </summary>
        [Optional] public long? levels { get; set; }

        /// <summary>
        /// Specifies details on how to format the stack frames.
        /// </summary>
        [Optional] public StackFrameFormat format { get; set; }
    }

}
