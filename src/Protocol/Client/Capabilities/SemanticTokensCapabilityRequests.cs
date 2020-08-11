using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SemanticTokensCapabilityRequests
    {
        /// <summary>
        /// The client will send the `textDocument/semanticTokens/range` request if
        /// the server provides a corresponding handler.
        /// </summary>
        [Optional]
        public Supports<SemanticTokensCapabilityRequestRange> Range { get; set; }

        /// <summary>
        /// The client will send the `textDocument/semanticTokens/full` request if
        /// the server provides a corresponding handler.
        /// </summary>
        [Optional]
        public Supports<SemanticTokensCapabilityRequestFull> Full { get; set; }
    }
}
