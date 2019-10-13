using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentSymbolClientCapabilities : DynamicCapability, ConnectedCapability<IDocumentSymbolHandler>
    {

        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `textDocument/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindClientCapabilities SymbolKind { get; set; }

        /// <summary>
		/// Whether document symbol supports hierarchical `DocumentSymbol`s.
        /// </summary>
        [Optional]
        public bool? HierarchicalDocumentSymbolSupport { get; set; }
    }
}
