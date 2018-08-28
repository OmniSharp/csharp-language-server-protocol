using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class PrepareRenameParams : TextDocumentPositionParams, IRequest<RangeOrPlaceholderRange>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        public Position Position { get; set; }
    }
}
