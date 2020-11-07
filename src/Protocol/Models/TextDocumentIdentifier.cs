using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record TextDocumentIdentifier(DocumentUri Uri)
    {
        public TextDocumentIdentifier(): this(null!) {}

        public static implicit operator TextDocumentIdentifier(Uri uri) => new TextDocumentIdentifier(uri);
        public static implicit operator TextDocumentIdentifier(DocumentUri uri) => new TextDocumentIdentifier(uri);
        public static implicit operator TextDocumentIdentifier(string uri) => new TextDocumentIdentifier(uri);

        // ReSharper disable once ConstantConditionalAccessQualifier
        private string DebuggerDisplay => Uri?.ToString() ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
