using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentSymbolCapability : DynamicCapability, ConnectedCapability<IDocumentSymbolHandler> {

        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `textDocument/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindCapability SymbolKind { get; set; }
    }
}
