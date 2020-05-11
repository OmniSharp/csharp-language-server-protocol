using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceSymbolCapability : DynamicCapability, ConnectedCapability<IWorkspaceSymbolsHandler>
    {
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public SymbolKindCapability SymbolKind { get; set; }

        /// <summary>
        /// The client supports tags on `SymbolInformation`.Tags are supported on
        /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
        /// Clients supporting tags have to handle unknown tags gracefully.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public Supports<TagSupportCapability> TagSupport { get; set; }
    }
}
