using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    public class BreakpointLocation
    {
        /// <summary>
        /// Start line of breakpoint location.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Optional start column of breakpoint location.
        /// </summary>
        [Optional]
        public int Column { get; set; }

        /// <summary>
        /// Optional end line of breakpoint location if the location covers a range.
        /// </summary>
        [Optional]
        public int EndLine { get; set; }

        /// <summary>
        /// Optional end column of breakpoint location if the location covers a range.
        /// </summary>
        [Optional]
        public int EndColumn { get; set; }
    }
}
