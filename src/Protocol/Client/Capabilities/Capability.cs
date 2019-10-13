using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class Capability
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        public WorkspaceCapability Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        public TextDocumentCapability TextDocument { get; set; }

        /// <summary>
        /// Window specific client capabilities.
        /// </summary>
        public WindowCapability Window { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();
    }
}
