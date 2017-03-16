using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Diagnostic
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The diagnostic's severity. Can be omitted. If omitted it is up to the
        /// client to interpret diagnostics as error, warning, info or hint.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DiagnosticSeverity? Severity { get; set; }

        /// <summary>
        /// The diagnostic's code. Can be omitted.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DiagnosticCode Code { get; set; }

        /// <summary>
        /// A human-readable string describing the source of this
        /// diagnostic, e.g. 'typescript' or 'super lint'.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        /// <summary>
        /// The diagnostic's message.
        /// </summary>
        public string Message { get; set; }
    }
}