using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A collection of document filters used to identify valid documents
    /// </summary>
    public class DocumentSelector : ContainerBase<DocumentFilter>
    {
        public DocumentSelector() : this(Enumerable.Empty<DocumentFilter>())
        {
        }

        public DocumentSelector(IEnumerable<DocumentFilter> items) : base(items)
        {
        }

        public DocumentSelector(params DocumentFilter[] items) : base(items)
        {
        }

        public static implicit operator DocumentSelector(DocumentFilter[] items)
        {
            return new DocumentSelector(items);
        }

        public static implicit operator DocumentSelector(Collection<DocumentFilter> items)
        {
            return new DocumentSelector(items);
        }

        public static implicit operator DocumentSelector(List<DocumentFilter> items)
        {
            return new DocumentSelector(items);
        }

        public static implicit operator string(DocumentSelector documentSelector)
        {
            return documentSelector != null ?
                string.Join(", ", documentSelector.Select(x => (string)x)) :
                null;
        }

        public bool IsMatch(TextDocumentAttributes attributes)
        {
            return this.Any(z => z.IsMatch(attributes));
        }

        public override string ToString()
        {
            return this;
        }

        public static DocumentSelector ForPattern(params string[] wildcards)
        {
            return new DocumentSelector(wildcards.Select(DocumentFilter.ForPattern));
        }

        public static DocumentSelector ForLanguage(params string[] languages)
        {
            return new DocumentSelector(languages.Select(DocumentFilter.ForLanguage));
        }

        public static DocumentSelector ForScheme(params string[] schemes)
        {
            return new DocumentSelector(schemes.Select(DocumentFilter.ForScheme));
        }
    }
}
