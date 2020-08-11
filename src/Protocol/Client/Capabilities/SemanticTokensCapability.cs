using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the `textDocument/semanticTokens`
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensCapability : DynamicCapability, ConnectedCapability<ISemanticTokensHandler>,
                                            ConnectedCapability<ISemanticTokensDeltaHandler>, ConnectedCapability<ISemanticTokensRangeHandler>
    {
        /// <summary>
        /// Which requests the client supports and might send to the server.
        /// </summary>
        public SemanticTokensCapabilityRequests Requests { get; set; }

        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        public Container<SemanticTokenType> TokenTypes { get; set; }

        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        public Container<SemanticTokenModifier> TokenModifiers { get; set; }

        /// <summary>
        /// The formats the clients supports.
        /// </summary>
        public Container<SemanticTokenFormat> Formats { get; set; }
    }
}
