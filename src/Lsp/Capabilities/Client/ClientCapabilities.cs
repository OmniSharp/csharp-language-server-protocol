using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public WorkspaceClientCapabilites Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentClientCapabilities TextDocument { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Experimental { get; set; } = new Dictionary<string, object>();
    }
}