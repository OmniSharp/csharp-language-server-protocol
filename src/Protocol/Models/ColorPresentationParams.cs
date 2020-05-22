using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.ColorPresentation, Direction.ClientToServer)]
    public class ColorPresentationParams : IRequest<Container<ColorPresentation>>
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
        /// <summary>
        /// The actual color value for this color range.
        /// </summary>
        public DocumentColor Color { get; set; }
        /// <summary>
        /// The range in the document where this color appers.
        /// </summary>
        public Range Range { get; set; }
    }
}
