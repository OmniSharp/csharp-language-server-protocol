using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensEdits
    {
        /// <summary>
        /// An optional result id. If provided and clients support delta updating
        /// the client will include the result id in the next semantic token request.
        /// A server can then instead of computing all semantic tokens again simply
        /// send a delta.
        /// </summary>
        [Optional]
        public string ResultId { get; set; }

        /// <summary>
        /// For a detailed description how these edits are structured pls see
        /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L131
        /// </summary>
        public Container<SemanticTokensEdit> Edits { get; set; }
    }
}
