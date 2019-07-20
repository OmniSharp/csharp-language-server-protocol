using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// Provides formatting information for a stack frame.
    /// </summary>
    public class StackFrameFormat : ValueFormat
    {
        /// <summary>
        /// Displays parameters for the stack frame.
        /// </summary>
        [Optional] public bool? Parameters { get; set; }

        /// <summary>
        /// Displays the types of parameters for the stack frame.
        /// </summary>
        [Optional] public bool? ParameterTypes { get; set; }

        /// <summary>
        /// Displays the names of parameters for the stack frame.
        /// </summary>
        [Optional] public bool? ParameterNames { get; set; }

        /// <summary>
        /// Displays the values of parameters for the stack frame.
        /// </summary>
        [Optional] public bool? ParameterValues { get; set; }

        /// <summary>
        /// Displays the line long of the stack frame.
        /// </summary>
        [Optional] public bool? Line { get; set; }

        /// <summary>
        /// Displays the module of the stack frame.
        /// </summary>
        [Optional] public bool? Module { get; set; }

        /// <summary>
        /// Includes all stack frames, including those the debug adapter might otherwise hide.
        /// </summary>
        [Optional] public bool? IncludeAll { get; set; }
    }
}