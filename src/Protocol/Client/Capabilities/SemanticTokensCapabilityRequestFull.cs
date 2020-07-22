using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// The client will send the `textDocument/semanticTokens/full` request if
    /// the server provides a corresponding handler.
    /// </summary>
    public class SemanticTokensCapabilityRequestFull
    {
        /// <summary>
        /// The client will send the `textDocument/semanticTokens/full/delta` request if
        /// the server provides a corresponding handler.
        /// </summary>
        [Optional]
        public bool Delta { get; set; }
    }
}