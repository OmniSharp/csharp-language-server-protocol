using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Disposables;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class TextDocumentIdentifiers : IEnumerable<ITextDocumentIdentifier>
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
