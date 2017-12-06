using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

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
}
