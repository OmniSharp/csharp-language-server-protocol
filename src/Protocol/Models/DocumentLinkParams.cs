using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DocumentLink)]
    public class DocumentLinkParams : ITextDocumentIdentifierParams, IRequest<DocumentLinkContainer>, IWorkDoneProgressParams, IPartialItems<DocumentLink>
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
