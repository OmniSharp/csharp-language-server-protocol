using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ColorPresentationParams : IRequest<Container<ColorPresentation>>
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
        /// <summary>
        /// The actual color value for this color range.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// The range in the document where this color appers.
        /// </summary>
        public Range Range { get; set; }
    }
}
