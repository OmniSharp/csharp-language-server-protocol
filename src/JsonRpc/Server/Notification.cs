using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Notification : IMethodWithParams, ITraceData
    {
        public Notification(
            string method,
            JToken? @params
        )
        {
            Method = method;
            Params = @params;
        }

        public string Method { get; }

        public JToken? Params { get; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>traceparent</c> value.
        /// </summary>
        public string? TraceParent { get; set; }

        /// <summary>
        /// Gets or sets the data for the <see href="https://www.w3.org/TR/trace-context/">W3C Trace Context</see> <c>tracestate</c> value.
        /// </summary>
        public string? TraceState { get; set; }
    }
}
