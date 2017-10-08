using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// A collection of document filters used to identify valid documents
    /// </summary>
    // [JsonConverter(typeof(DocumentSelectorConverter))]
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

        public static explicit operator string(DocumentSelector documentSelector)
        {
            return string.Join(", ", documentSelector.Select(x => (string)x));
        }

        public bool IsMatch(TextDocumentAttributes attributes)
        {
            return this.Any(z => z.IsMatch(attributes));
        }
    }
}
