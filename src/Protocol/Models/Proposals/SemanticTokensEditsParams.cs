using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.SemanticTokensEdits, Direction.ClientToServer)]
    public class SemanticTokensEditsParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
        IPartialItemRequest<SemanticTokensOrSemanticTokensEdits, SemanticTokensPartialResultOrSemanticTokensEditsPartialResult>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The previous result id.
        /// </summary>
        public string PreviousResultId { get; set; }

        public ProgressToken WorkDoneToken { get; set; }
        public ProgressToken PartialResultToken { get; set; }
    }
}
