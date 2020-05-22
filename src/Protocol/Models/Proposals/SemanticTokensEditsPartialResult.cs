using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensEditsPartialResult
    {
        /// <summary>
        /// For a detailed description how these edits are structured pls see
        /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L131
        /// </summary>
        public Container<SemanticTokensEdit> Edits { get; set; }
    }
}
