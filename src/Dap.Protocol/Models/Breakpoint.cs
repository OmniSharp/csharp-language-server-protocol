using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Information about a Breakpoint created in setBreakpoints or setFunctionBreakpoints.
    /// </summary>

    public class Breakpoint
    {
        /// <summary>
        /// An optional identifier for the breakpoint. It is needed if breakpoint events are used to update or remove breakpoints.
        /// </summary>
        [Optional] public long? Id { get; set; }

        /// <summary>
        /// If true breakpoint could be set (but not necessarily at the desired location).
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// An optional message about the state of the breakpoint. This is shown to the user and can be used to explain why a breakpoint could not be verified.
        /// </summary>
        [Optional] public string Message { get; set; }

        /// <summary>
        /// The source where the breakpoint is located.
        /// </summary>
        [Optional] public Source Source { get; set; }

        /// <summary>
        /// The start line of the actual range covered by the breakpoint.
        /// </summary>
        [Optional] public int? Line { get; set; }

        /// <summary>
        /// An optional start column of the actual range covered by the breakpoint.
        /// </summary>
        [Optional] public int? Column { get; set; }

        /// <summary>
        /// An optional end line of the actual range covered by the breakpoint.
        /// </summary>
        [Optional] public int? EndLine { get; set; }

        /// <summary>
        /// An optional end column of the actual range covered by the breakpoint. If no end line is given, then the end column is assumed to be in the start line.
        /// </summary>
        [Optional] public int? EndColumn { get; set; }
    }
}
