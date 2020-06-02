using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkContainer<TData> : Container<DocumentLink<TData>>, IAggregateResults
        where TData : CanBeResolvedData
    {
        public DocumentLinkContainer() : this(Enumerable.Empty<DocumentLink<TData>>())
        {
        }

        public DocumentLinkContainer(IEnumerable<DocumentLink<TData>> items) : base(items)
        {
        }

        public DocumentLinkContainer(params DocumentLink<TData>[] items) : base(items)
        {
        }

        public static implicit operator DocumentLinkContainer<TData>(DocumentLink<TData>[] items)
        {
            return new DocumentLinkContainer<TData>(items);
        }

        public static implicit operator DocumentLinkContainer<TData>(Collection<DocumentLink<TData>> items)
        {
            return new DocumentLinkContainer<TData>(items);
        }

        public static implicit operator DocumentLinkContainer<TData>(List<DocumentLink<TData>> items)
        {
            return new DocumentLinkContainer<TData>(items);
        }

        object IAggregateResults.AggregateResults(IEnumerable<object> values)
        {
            return new DocumentLinkContainer<CanBeResolvedData>(
                values
                    .Cast<IEnumerable<object>>()
                    .SelectMany(z => z.OfType<IDocumentLink<CanBeResolvedData>>())
                    .Concat(this)
                    .Select(DocumentLink<CanBeResolvedData>.From)
            );
        }
    }
}
