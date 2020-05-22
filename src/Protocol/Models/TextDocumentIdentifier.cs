using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentIdentifier : IEquatable<TextDocumentIdentifier>
    {
        public TextDocumentIdentifier()
        {

        }

        public TextDocumentIdentifier(DocumentUri uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// The text document's URI.
        /// </summary>
        public DocumentUri Uri { get; set; }

        public bool Equals(TextDocumentIdentifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uri?.Equals(other.Uri) == true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TextDocumentIdentifier) obj);
        }

        public override int GetHashCode() => Uri.GetHashCode();

        public static bool operator ==(TextDocumentIdentifier left, TextDocumentIdentifier right) => Equals(left, right);

        public static bool operator !=(TextDocumentIdentifier left, TextDocumentIdentifier right) => !Equals(left, right);

        public static implicit operator TextDocumentIdentifier(DocumentUri uri)
        {
            return new TextDocumentIdentifier() {Uri = uri};
        }

        public static implicit operator TextDocumentIdentifier(string uri)
        {
            return new TextDocumentIdentifier() {Uri = uri};
        }
    }
}
