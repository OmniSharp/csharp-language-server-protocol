using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ITextDocumentIdentifier
    {
        /// <summary>
        /// Returns the attributes for the document at the given URI.  This can return null.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        TextDocumentAttributes GetTextDocumentAttributes(Uri uri);
    }

    public abstract class TextDocumentIdentifierBase : ITextDocumentIdentifier
    {
        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            var (languageId, schema) = GetAttributes(uri);
            return new TextDocumentAttributes(uri, languageId, schema);
        }

        protected abstract (string languageId, string schema) GetAttributes(Uri uri);
    }
}
