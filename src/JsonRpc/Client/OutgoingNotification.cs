using Newtonsoft.Json;

namespace OmniSharp.Extensions.JsonRpc.Client
{
    public class OutgoingNotification : ITraceData
    {
        public string Method { get; set; } = null!;

        public object? Params { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>traceparent</c> value.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? TraceParent { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>tracestate</c> value.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? TraceState { get; set; }
    }
}
