using System;
using System.Collections.Immutable;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensEdit
    {
        /// <summary>
        /// The start index of the edit
        /// </summary>
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        public int Start { get; set; }

        /// <summary>
        /// The number of items to delete
        /// </summary>
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        public int DeleteCount { get; set; }

        /// <summary>
        /// The actual tokens. For a detailed description about how the data is
        /// structured pls see
        /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
        /// </summary>
        /// <remarks>
        /// <see cref="uint"/> in the LSP spec
        /// </remarks>
        [Optional]
        public ImmutableArray<int>? Data { get; set; } = ImmutableArray<int>.Empty;
    }
}
