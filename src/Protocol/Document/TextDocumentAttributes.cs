using System;
using System.Collections.Generic;

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
        public string Scheme { get; }
        public string LanguageId { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TextDocumentAttributes);
        }

        public bool Equals(TextDocumentAttributes other)
        {
            return other != null &&
                   DocumentUri.Comparer.Equals(Uri, other.Uri) &&
                   Scheme == other.Scheme &&
                   LanguageId == other.LanguageId;
        }

        public override int GetHashCode()
        {
            var hashCode = -918855467;
            hashCode = hashCode * -1521134295 + DocumentUri.Comparer.GetHashCode(Uri);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Scheme);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LanguageId);
            return hashCode;
        }

        public static bool operator ==(TextDocumentAttributes attributes1, TextDocumentAttributes attributes2)
        {
            return EqualityComparer<TextDocumentAttributes>.Default.Equals(attributes1, attributes2);
        }

        public static bool operator !=(TextDocumentAttributes attributes1, TextDocumentAttributes attributes2)
        {
            return !(attributes1 == attributes2);
        }
    }
}
