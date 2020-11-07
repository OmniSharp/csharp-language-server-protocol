using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record TextDocumentItem(DocumentUri Uri, string LanguageId) : TextDocumentIdentifier(Uri)
    {
        public TextDocumentItem(): this(languageId: null!, uri:null!) {}
        public TextDocumentItem(string languageId, DocumentUri uri) : this(uri, languageId) { }

        /// <summary>
        /// The version number of this document (it will strictly increase after each
        /// change, including undo/redo).
        /// </summary>
        public int Version { get; init; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        public string? Text { get; init; }

        private string DebuggerDisplay => $"({LanguageId}@{Version}) {Uri}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
