using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.SemanticTokensFull, Direction.ClientToServer)]
    public class SemanticTokensParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                        IPartialItemRequest<SemanticTokens, SemanticTokensPartialResult>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        public ProgressToken WorkDoneToken { get; set; }
        public ProgressToken PartialResultToken { get; set; }
    }
}
