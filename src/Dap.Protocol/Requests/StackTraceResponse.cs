using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StackTraceResponse
    {
        /// <summary>
        /// The frames of the stackframe.If the array has length zero, there are no stackframes available.
        /// This means that there is no location information available.
        /// </summary>
        public Container<StackFrame> StackFrames { get; set; }

        /// <summary>
        /// The total number of frames available.
        /// </summary>
        [Optional] public long? TotalFrames { get; set; }

    }

}
