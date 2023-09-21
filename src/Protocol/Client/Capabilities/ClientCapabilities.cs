using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ClientCapabilities : CapabilitiesBase, IClientCapabilities
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        [Optional]
        public WorkspaceClientCapabilities? Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        [Optional]
        public TextDocumentClientCapabilities? TextDocument { get; set; }

        /// <summary>
        /// Capabilities specific to the notebook document support.
        ///
        /// @since 3.17.0
        /// </summary>
        [Optional]
        public NotebookDocumentClientCapabilities? NotebookDocument { get; set; }

        /// <summary>
        /// Window specific client capabilities.
        /// </summary>
        [Optional]
        public WindowClientCapabilities? Window { get; set; }

        /// <summary>
        /// General client capabilities.
        /// </summary>
        [Optional]
        public GeneralClientCapabilities? General { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        public IDictionary<string, JToken> Experimental { get; set; } = new Dictionary<string, JToken>();
    }
}
