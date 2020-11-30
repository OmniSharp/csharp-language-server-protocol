using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ClientCapabilities : CapabilitiesBase
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

    /// <summary>
    /// General client capabilities.
    /// </summary>
    public class GeneralClientCapabilities
    {
        /// <summary>
        /// Client capabilities specific to regular expressions.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public RegularExpressionsClientCapabilities? RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional] public MarkdownClientCapabilities? Markdown { get; set; }
    }
}
