using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentColorParams : IRequest<Container<ColorInformation>>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
