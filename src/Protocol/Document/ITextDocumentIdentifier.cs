namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ITextDocumentIdentifier
    {
        /// <summary>
        /// Returns the attributes for the document at the given URI.  This can return null.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri);
    }

    public abstract class TextDocumentIdentifierBase : ITextDocumentIdentifier
    {
        public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            var (languageId, schema) = GetAttributes(uri);
            return new TextDocumentAttributes(uri, languageId, schema);
        }

        protected abstract (string languageId, string schema) GetAttributes(DocumentUri uri);
    }
}
