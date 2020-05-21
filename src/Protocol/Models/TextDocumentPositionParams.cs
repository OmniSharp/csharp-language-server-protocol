namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentPositionParams : ITextDocumentIdentifierParams
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
