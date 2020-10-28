using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the semantic token requests scoped to the
    /// workspace.
    ///
    /// @since 3.16.0 - proposed state.
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.SemanticTokens))]
    public class SemanticTokensWorkspaceCapability : ICapability
    {
        /// <summary>
        /// Whether the client implementation supports a refresh request send from
        /// the server to the client. This is useful if a server detects a project
        /// wide configuration change which requires a re-calculation of all semantic
        /// tokens provided by the server issuing the request.
        /// </summary>
        [Optional]
        public bool RefreshSupport { get; set; }
    }
}
