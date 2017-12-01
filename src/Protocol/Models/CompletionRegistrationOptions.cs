using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CompletionRegistrationOptions : TextDocumentRegistrationOptions, ICompletionOptions
    {
        /// <summary>
        /// The characters that trigger completion automatically.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<string> TriggerCharacters { get; set; }

        /// <summary>
        /// The server provides support to resolve additional
        /// information for a completion item.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ResolveProvider { get; set; }
    }
}