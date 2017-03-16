using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    /// <summary>
    /// Represents a parameter of a callable-signature. A parameter can
    /// have a label and a doc-comment.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ParameterInformation
    {
        /// <summary>
        /// The label of this parameter. Will be shown in
        /// the UI.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The human-readable doc-comment of this parameter. Will be shown
        /// in the UI but can be omitted.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Documentation { get; set; }
    }
}