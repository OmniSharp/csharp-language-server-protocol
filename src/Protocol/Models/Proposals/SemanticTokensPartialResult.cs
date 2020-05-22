using System;
using System.Collections.Immutable;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensPartialResult
    {
        /// <summary>
        /// The actual tokens. For a detailed description about how the data is
        /// structured pls see
        /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
        /// </summary>
        public ImmutableArray<int> Data { get; set; }
    }
}
