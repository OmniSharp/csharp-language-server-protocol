using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class DataBreakpointInfoResponse
    {
        /// <summary>
        /// An identifier for the data on which a data breakpoint can be registered with the setDataBreakpoints request or null if no data breakpoint is available.
        /// </summary>
        public string dataId { get; set; }

        /// <summary>
        /// UI string that describes on what data the breakpoint is set on or why a data breakpoint is not available.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Optional attribute listing the available access types for a potential data breakpoint.A UI frontend could surface this information.
        /// </summary>
        [Optional] public Container<DataBreakpointAccessType> accessTypes { get; set; }

        /// <summary>
        /// Optional attribute indicating that a potential data breakpoint could be persisted across sessions.
        /// </summary>
        [Optional] public bool? canPersist { get; set; }
    }

}
