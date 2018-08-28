using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class FoldingRangeRequestParam : ITextDocumentIdentifierParams, IRequest<Container<FoldingRange>>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
