using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the `textDocument/callHierarchy`.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyCapability : DynamicCapability, ConnectedCapability<ICallHierarchyHandler>,
        ConnectedCapability<ICallHierarchyIncomingHandler>, ConnectedCapability<ICallHierarchyOutgoingHandler>
    {
    }

    /// <summary>
    /// Capabilities specific to the `textDocument/semanticTokens`
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensCapability : DynamicCapability
    {
        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        public Container<string> TokenTypes { get; set; }

        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        public Container<string> TokenModifiers { get; set; }
    }
}
