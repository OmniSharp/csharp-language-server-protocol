using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ColorInformation
    {
        /// <summary>
        /// The range in the document where this color appers.
        /// </summary>
        public Range Range { get; set; }
        /// <summary>
        /// The actual color value for this color range.
        /// </summary>
        public Color Color { get; set; }
    }

    public class ColorPresentationParams
    {
        /// <summary>
        /// The document to provide document links for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
        /// <summary>
        /// The actual color value for this color range.
        /// </summary>
        public Color ColorInfo { get; set; }
        /// <summary>
        /// The range in the document where this color appers.
        /// </summary>
        public Range Range { get; set; }
    }

    public class ColorPresentation
    {
        /// <summary>
        /// The label of this color presentation. It will be shown on the color
        /// picker header. By default this is also the text that is inserted when selecting
        /// this color presentation.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// An [edit](#TextEdit) which is applied to a document when selecting
        /// this presentation for the color.  When `falsy` the [label](#ColorPresentation.label)
        /// is used.
        /// </summary>
        [Optional]
        public TextEdit TextEdit { get; set; }
        /// <summary>
        /// An optional array of additional [text edits](#TextEdit) that are applied when
        /// selecting this color presentation. Edits must not overlap with the main [edit](#ColorPresentation.textEdit) nor with themselves.
        /// </summary>
        [Optional]
        public Container<TextEdit> AdditionalTextEdits { get; set; }
    }
}
