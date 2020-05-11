using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>FunctionBreakpoint
    /// Properties of a breakpoint passed to the setFunctionBreakpoints request.
    /// </summary>
    public class FunctionBreakpoint
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional expression for conditional breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Condition { get; set; }

        /// <summary>
        /// An optional expression that controls how many hits of the breakpoint are ignored. The backend is expected to interpret the expression as needed.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string HitCondition { get; set; }
    }
}
