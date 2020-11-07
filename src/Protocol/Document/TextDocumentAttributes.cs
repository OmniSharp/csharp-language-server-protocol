using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public record TextDocumentAttributes
    {
        public TextDocumentAttributes() { }

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

        public DocumentUri Uri { get;init; }
        public string? Scheme { get; init; }
        public string LanguageId { get;init; }
    }
}
