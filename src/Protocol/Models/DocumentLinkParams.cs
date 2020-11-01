using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DocumentLink, Direction.ClientToServer)]
    public class DocumentLinkParams : ITextDocumentIdentifierParams, IPartialItemsRequest<DocumentLinkContainer, DocumentLink>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; } = null!;

        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
