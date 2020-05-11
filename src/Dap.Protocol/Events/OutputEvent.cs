using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class OutputEvent : IRequest
    {
        /// <summary>
        /// The output category. If not specified, 'console' is assumed.
        /// Values: 'console', 'stdout', 'stderr', 'telemetry', etc.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Category { get; set; }

        /// <summary>
        /// The output to report.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// If an attribute 'variablesReference' exists and its value is > 0, the output contains objects which can be retrieved by passing 'variablesReference' to the 'variables' request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? VariablesReference { get; set; }

        /// <summary>
        /// An optional source location where the output was produced.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Source Source { get; set; }

        /// <summary>
        /// An optional source location line where the output was produced.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? Line { get; set; }

        /// <summary>
        /// An optional source location column where the output was produced.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? Column { get; set; }

        /// <summary>
        /// Optional data to report. For the 'telemetry' category the data will be sent to telemetry, for the other categories the data is shown in JSON format.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public JsonElement Data { get; set; }
    }

}
