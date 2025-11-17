using System.Collections;
using System.Reactive.Disposables;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public interface INotebookDocumentIdentifier
    {
        /// <summary>
        /// Returns the attributes for the document at the given URI.  This can return null.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        NotebookDocumentAttributes GetNotebookDocumentAttributes(DocumentUri uri);
    }

    public abstract class NotebookDocumentIdentifierBase : INotebookDocumentIdentifier
    {
        public NotebookDocumentAttributes GetNotebookDocumentAttributes(DocumentUri uri)
        {
            var (languageId, schema) = GetAttributes(uri);
            return new NotebookDocumentAttributes(uri, languageId, schema);
        }

        protected abstract (string notebookType, string schema) GetAttributes(DocumentUri uri);
    }

    public class NotebookDocumentIdentifiers : IEnumerable<INotebookDocumentIdentifier>
    {
        private readonly HashSet<INotebookDocumentIdentifier> _NotebookDocumentIdentifiers = new HashSet<INotebookDocumentIdentifier>();
        public IEnumerator<INotebookDocumentIdentifier> GetEnumerator() => _NotebookDocumentIdentifiers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IDisposable Add(params INotebookDocumentIdentifier[] identifiers)
        {
            foreach (var item in identifiers)
                _NotebookDocumentIdentifiers.Add(item);
            return Disposable.Create(
                () => {
                    foreach (var NotebookDocumentIdentifier in identifiers)
                    {
                        _NotebookDocumentIdentifiers.Remove(NotebookDocumentIdentifier);
                    }
                }
            );
        }
    }
}
