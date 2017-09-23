using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// Represents the signature of something callable. A signature
    /// can have a label, like a function-name, a doc-comment, and
    /// a set of parameters.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SignatureInformation
    {
        /// <summary>
        /// The label of this signature. Will be shown in
        /// the UI.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The human-readable doc-comment of this signature. Will be shown
        /// in the UI but can be omitted.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Documentation { get; set; }

        /// <summary>
        /// The parameters of this signature.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<ParameterInformation> Parameters { get; set; }
    }
}