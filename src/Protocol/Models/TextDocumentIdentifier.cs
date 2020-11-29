using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record TextDocumentIdentifier
    {
        public TextDocumentIdentifier() { }

        public TextDocumentIdentifier(DocumentUri uri) => Uri = uri;

        /// <summary>
        /// The text document's URI.
        /// </summary>
        public DocumentUri Uri { get; init; }

        public static implicit operator TextDocumentIdentifier(DocumentUri uri) => new TextDocumentIdentifier { Uri = uri };

        public static implicit operator TextDocumentIdentifier(string uri) => new TextDocumentIdentifier { Uri = uri };

        // ReSharper disable once ConstantConditionalAccessQualifier
        private string DebuggerDisplay => Uri?.ToString() ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
