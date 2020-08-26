using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class TextDocumentIdentifier : IEquatable<TextDocumentIdentifier>
    {
        public TextDocumentIdentifier()
        {
        }

        public TextDocumentIdentifier(DocumentUri uri) => Uri = uri;

        /// <summary>
        /// The text document's URI.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        public bool Equals(TextDocumentIdentifier? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uri.Equals(other.Uri);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TextDocumentIdentifier) obj);
        }

        public override int GetHashCode() => Uri.GetHashCode();

        public static bool operator ==(TextDocumentIdentifier left, TextDocumentIdentifier right) => Equals(left, right);

        public static bool operator !=(TextDocumentIdentifier left, TextDocumentIdentifier right) => !Equals(left, right);

        public static implicit operator TextDocumentIdentifier(DocumentUri uri) => new TextDocumentIdentifier { Uri = uri };

        public static implicit operator TextDocumentIdentifier(string uri) => new TextDocumentIdentifier { Uri = uri };

        private string DebuggerDisplay => Uri?.ToString()!;

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
