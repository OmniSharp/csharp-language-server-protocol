using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the code lens requests scoped to the
    /// workspace.
    ///
    /// @since 3.16.0 - proposed state.
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.CodeLens))]
    public class CodeLensWorkspaceClientCapabilities
    {
        /// <summary>
	 /// Whether the client implementation supports a refresh request send from the server
	 /// to the client. This is useful if a server detects a change which requires a
	 /// re-calculation of all code lenses.
         /// </summary>
        [Optional]
        public bool RefreshSupport { get; set; }
    }
}
