using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        public WorkspaceClientCapabilites Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        public TextDocumentClientCapabilities TextDocument { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();
    }
}
