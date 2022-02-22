using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    public record ExceptionFilterOptions
    {
        /// <summary>
        /// ID of an exception filter returned by the 'exceptionBreakpointFilters'
        /// capability.
        /// </summary>
        public string FilterId { get; init; } = null!;

        /// <summary>
        /// An optional expression for conditional exceptions.
        /// The exception will break into the debugger if the result of the condition
        /// is true.
        /// </summary>
        [Optional]
        public string? Condition { get; init; }
    }
}
