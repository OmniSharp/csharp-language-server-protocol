using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public class TextDocumentAttributes
    {
        public TextDocumentAttributes(Uri uri, string languageId)
        {
            Uri = uri;
            Scheme = uri.Scheme;
            LanguageId = languageId;
        }

        public TextDocumentAttributes(Uri uri, string scheme, string languageId)
        {
            Uri = uri;
            Scheme = scheme;
            LanguageId = languageId;
        }

        public Uri Uri { get; }
        public string Scheme { get; }
        public string LanguageId { get; }
    }
}