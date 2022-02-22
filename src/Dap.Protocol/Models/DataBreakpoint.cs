using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Properties of a data breakpoint passed to the setDataBreakpoints request.
    /// </summary>
    public record DataBreakpoint
    {
        /// <summary>
        /// An id representing the data. This id is returned from the dataBreakpointInfo request.
        /// </summary>
        public string DataId { get; init; } = null!;

        /// <summary>
        /// The access type of the data.
        /// </summary>
        [Optional]
        public DataBreakpointAccessType? AccessType { get; init; }

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
    }
}
