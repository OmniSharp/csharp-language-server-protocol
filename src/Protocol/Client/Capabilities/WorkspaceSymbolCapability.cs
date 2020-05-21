using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceSymbolCapability : DynamicCapability, ConnectedCapability<IWorkspaceSymbolsHandler>
    {
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindCapability SymbolKind { get; set; }

        /// <summary>
        /// The client supports tags on `SymbolInformation`.Tags are supported on
        /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
        /// Clients supporting tags have to handle unknown tags gracefully.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Optional]
        public Supports<TagSupportCapability> TagSupport { get; set; }
    }
}
