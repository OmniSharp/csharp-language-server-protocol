using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  An event describing a change to a text document. If range and rangeLength are omitted
    ///  the new text is considered to be the full content of the document.
    /// </summary>
    public class TextDocumentContentChangeEvent
    {
        /// <summary>
        ///  The range of the document that changed.
        /// </summary>
        [Optional]
        public Range Range { get; set; }

        /// <summary>
        ///  The length of the range that got replaced.
        /// </summary>
        [Optional]
        public int RangeLength { get; set; }

        /// <summary>
        ///  The new text of the document.
        /// </summary>
        public string Text { get; set; }
    }
}
