using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Capabilities specific to the `textDocument/semanticTokens`
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.SemanticTokens))]
    public class SemanticTokensCapability : DynamicCapability, ConnectedCapability<ISemanticTokensHandler>,
                                            ConnectedCapability<ISemanticTokensDeltaHandler>, ConnectedCapability<ISemanticTokensRangeHandler>
    {
        /// <summary>
        /// Which requests the client supports and might send to the server.
        /// </summary>
        public SemanticTokensCapabilityRequests Requests { get; set; } = null!;

        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        public Container<SemanticTokenType> TokenTypes { get; set; } = null!;

        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        public Container<SemanticTokenModifier> TokenModifiers { get; set; } = null!;

        /// <summary>
        /// The formats the clients supports.
        /// </summary>
        public Container<SemanticTokenFormat> Formats { get; set; } = null!;

        /// <summary>
        /// Whether the client supports tokens that can overlap each other.
        /// </summary>
        [Optional]
        public bool OverlappingTokenSupport { get; set; }

        /// <summary>
        /// Whether the client supports tokens that can span multiple lines.
        /// </summary>
        [Optional]
        public bool MultilineTokenSupport { get; set; }
    }
}
