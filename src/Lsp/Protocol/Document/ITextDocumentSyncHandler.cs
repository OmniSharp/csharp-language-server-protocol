using System;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler, IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler
    {
        TextDocumentSyncOptions Options { get; }
        TextDocumentAttributes GetTextDocumentAttributes(Uri uri);
    }

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
