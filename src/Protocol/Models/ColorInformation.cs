namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ColorInformation
    {
        /// <summary>
        /// The range in the document where this color appers.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The actual color value for this color range.
        /// </summary>
        public DocumentColor Color { get; set; } = null!;
    }
}
