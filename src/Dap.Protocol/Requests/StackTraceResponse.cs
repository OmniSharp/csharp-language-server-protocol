using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StackTraceResponse
    {
        /// <summary>
        /// The frames of the stackframe.If the array has length zero, there are no stackframes available.
        /// This means that there is no location information available.
        /// </summary>
        public Container<StackFrame> stackFrames { get; set; }

        /// <summary>
        /// The total number of frames available.
        /// </summary>
        [Optional] public long? totalFrames { get; set; }

    }

}
