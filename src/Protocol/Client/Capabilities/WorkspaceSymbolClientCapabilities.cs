using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceSymbolClientCapabilities : DynamicCapability, ConnectedCapability<IWorkspaceSymbolsHandler>
    {
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindClientCapabilities SymbolKind { get; set; }
    }
}
