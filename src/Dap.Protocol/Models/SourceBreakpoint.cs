using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// SourceBreakpoint
    /// Properties of a breakpoint or logpoint passed to the setBreakpoints request.
    /// </summary>
    public record SourceBreakpoint
    {
        /// <summary>
        /// The source line of the breakpoint or logpoint.
        /// </summary>
        public int Line { get; init; }

        /// <summary>
        /// An optional source column of the breakpoint.
        /// </summary>
        [Optional]
        public int? Column { get; init; }

        /// <summary>
        /// An optional expression for conditional breakpoints.
        /// </summary>
        [Optional]
        public string? Condition { get; init; }

        /// <summary>
        /// An optional expression that controls how many hits of the breakpoint are ignored. The backend is expected to interpret the expression as needed.
        /// </summary>
        [Optional]
        public string? HitCondition { get; init; }

        /// <summary>
        /// If this attribute exists and is non-empty, the backend must not 'break' (stop) but log the message instead. Expressions within {} are interpolated.
        /// </summary>
        [Optional]
        public string? LogMessage { get; init; }
    }
}
