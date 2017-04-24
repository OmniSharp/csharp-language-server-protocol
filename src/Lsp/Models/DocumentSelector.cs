using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lsp.Converters;
using Lsp.Protocol;
using Newtonsoft.Json;

namespace Lsp.Models
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

        public bool IsMatch(TextDocumentAttributes attributes)
        {
            return this.Any(z => z.IsMatch(attributes));
        }
    }
}