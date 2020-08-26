namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentEdit
    {
        /// <summary>
        /// The text document to change.
        /// </summary>
        public VersionedTextDocumentIdentifier TextDocument { get; set; } = null!;

        /// <summary>
        /// The edits to be applied.
        /// </summary>
        public TextEditContainer Edits { get; set; } = null!;
    }
}
