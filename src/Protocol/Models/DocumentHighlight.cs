namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A document highlight is a range inside a text document which deserves
    /// special attention. Usually a document highlight is visualized by changing
    /// the background color of its range.
    ///
    /// </summary>
    public class DocumentHighlight
    {
        /// <summary>
        /// The range this highlight applies to.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The highlight kind, default is DocumentHighlightKind.Text.
        /// </summary>
        public DocumentHighlightKind Kind { get; set; }
    }
}
