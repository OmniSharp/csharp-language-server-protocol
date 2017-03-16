using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Server
{
    /// <summary>
    ///  Format document on type options
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentOnTypeFormattingOptions
    {
        /// <summary>
        ///  A character on which formatting should be triggered, like `}`.
        /// </summary>
        public string FirstTriggerCharacter { get; set; }

        /// <summary>
        ///  More trigger characters.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<string> MoreTriggerCharacter { get; set; }
    }
}