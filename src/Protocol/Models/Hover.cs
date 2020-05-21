using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The result of a hover request.
    /// </summary>
    public class Hover
    {
        /// <summary>
        /// The hover's content
        /// </summary>
        public MarkedStringsOrMarkupContent Contents { get; set; }

        /// <summary>
        /// An optional range is a range inside a text document
        /// that is used to visualize a hover, e.g. by changing the background color.
        /// </summary>
        [Optional]
        public Range Range { get; set; }
    }
}
