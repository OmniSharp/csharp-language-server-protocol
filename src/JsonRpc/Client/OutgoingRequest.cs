using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public record OutgoingRequest : ITraceData
    {
        public object? Id { get; set; }

        public string? Method { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Params { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>traceparent</c> value.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceParent { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>tracestate</c> value.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceState { get; set; }
    }
}
