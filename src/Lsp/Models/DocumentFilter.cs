using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentFilter
    {
        /// <summary>
        /// A language id, like `typescript`.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        /// <summary>
        /// does the document filter contains a language
        /// </summary>
        [JsonIgnore]
        public bool HasLanguage => Language != null;

        /// <summary>
        /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Scheme { get; set; }

        /// <summary>
        /// does the document filter contains a scheme
        /// </summary>
        [JsonIgnore]
        public bool HasScheme => Scheme != null;

        /// <summary>
        /// A glob pattern, like `*.{ts,js}`.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Pattern { get; set; }

        /// <summary>
        /// does the document filter contains a paattern
        /// </summary>
        [JsonIgnore]
        public bool HasPattern => Pattern != null;
    }
}