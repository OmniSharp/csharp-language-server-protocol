using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public class TextDocumentAttributes : IEquatable<TextDocumentAttributes>
    {
        public TextDocumentAttributes(DocumentUri uri, string languageId)
        {
            Uri = uri;
            LanguageId = languageId;
        }

        public TextDocumentAttributes(DocumentUri uri, string scheme, string languageId)
        {
            Uri = uri;
            Scheme = scheme;
            LanguageId = languageId;
        }

        public DocumentUri Uri { get; }
        public string? Scheme { get; }
        public string LanguageId { get; }

        public bool Equals(TextDocumentAttributes? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uri.Equals(other.Uri) && Scheme == other.Scheme && LanguageId == other.LanguageId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TextDocumentAttributes) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Uri.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ ( Scheme != null ? Scheme.GetHashCode() : 0 );
                hashCode = ( hashCode * 397 ) ^ LanguageId.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TextDocumentAttributes? left, TextDocumentAttributes? right) => Equals(left, right);

        public static bool operator !=(TextDocumentAttributes? left, TextDocumentAttributes? right) => !Equals(left, right);
    }
}
