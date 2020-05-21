using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
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

    public class TextDocumentIdentifiers : IEnumerable<ITextDocumentIdentifier>
    {
        private readonly HashSet<ITextDocumentIdentifier> _textDocumentIdentifiers = new HashSet<ITextDocumentIdentifier>();
        public IEnumerator<ITextDocumentIdentifier> GetEnumerator() => _textDocumentIdentifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IDisposable Add(params ITextDocumentIdentifier[] identifiers)
        {
            foreach (var item in identifiers)
                _textDocumentIdentifiers.Add(item);
            return Disposable.Create(() => {
                foreach (var textDocumentIdentifier in identifiers)
                {
                    _textDocumentIdentifiers.Remove(textDocumentIdentifier);
                }
            });
        }
    }
}
