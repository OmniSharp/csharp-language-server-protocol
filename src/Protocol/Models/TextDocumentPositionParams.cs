namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public record TextDocumentPositionParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; init; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        public Position Position { get; init; }
    }
}
