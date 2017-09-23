using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// The result of a hover request.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Hover
    {
        /// <summary>
        /// The hover's content
        /// </summary>
        public MarkedStringContainer Contents { get; set; }

        /// <summary>
        /// An optional range is a range inside a text document
        /// that is used to visualize a hover, e.g. by changing the background color.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Range Range { get; set; }
    }
}