using Newtonsoft.Json;

namespace Lsp.Capabilities.Client
{
    public class CompletionCapability : DynamicCapability
    {
        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CompletionItemCapability CompletionItem { get; set; }
    }
}