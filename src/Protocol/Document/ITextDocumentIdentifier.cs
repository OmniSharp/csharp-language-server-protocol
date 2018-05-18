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
}