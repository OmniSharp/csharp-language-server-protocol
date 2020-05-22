using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        public WorkspaceClientCapabilities Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        public TextDocumentClientCapabilities TextDocument { get; set; }

        /// <summary>
        /// Window specific client capabilities.
        /// </summary>
        public WindowClientCapabilities Window { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();
    }
}
