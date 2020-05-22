using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.StackTrace, Direction.ClientToServer)]
    public class StackTraceArguments : IRequest<StackTraceResponse>
    {
        /// <summary>
        /// Retrieve the stacktrace for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// The index of the first frame to return; if omitted frames start at 0.
        /// </summary>
        [Optional] public long? StartFrame { get; set; }

        /// <summary>
        /// The maximum number of frames to return. If levels is not specified or 0, all frames are returned.
        /// </summary>
        [Optional] public long? Levels { get; set; }

        /// <summary>
        /// Specifies details on how to format the stack frames.
        /// </summary>
        [Optional] public StackFrameFormat Format { get; set; }
    }

}
