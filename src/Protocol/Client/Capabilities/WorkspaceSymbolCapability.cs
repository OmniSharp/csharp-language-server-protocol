using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceSymbolCapability : DynamicCapability, ConnectedCapability<IWorkspaceSymbolsHandler>
    {
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindCapability SymbolKind { get; set; }
    }

    public class SymbolKindCapability
    {
        /// <summary>
        /// The symbol kind values the client supports. When this
        /// property exists the client also guarantees that it will
        /// handle values outside its set gracefully and falls back
        /// to a default value when unknown.
        ///
        /// If this property is not present the client only supports
        /// the symbol kinds from `File` to `Array` as defined in
        /// the initial version of the protocol.
        /// </summary>
        [Optional]
        public Container<SymbolKind> ValueSet { get; set; }
    }
}
