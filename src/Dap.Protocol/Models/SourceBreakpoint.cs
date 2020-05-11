using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>SourceBreakpoint
    /// Properties of a breakpoint or logpoint passed to the setBreakpoints request.
    /// </summary>
    public class SourceBreakpoint
    {
        /// <summary>
        /// The source line of the breakpoint or logpoint.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// An optional source column of the breakpoint.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public int? Column { get; set; }

        /// <summary>
        /// An optional expression for conditional breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Condition { get; set; }

        /// <summary>
        /// An optional expression that controls how many hits of the breakpoint are ignored. The backend is expected to interpret the expression as needed.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string HitCondition { get; set; }

        /// <summary>
        /// If this attribute exists and is non-empty, the backend must not 'break' (stop) but log the message instead. Expressions within {} are interpolated.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string LogMessage { get; set; }
    }
}
