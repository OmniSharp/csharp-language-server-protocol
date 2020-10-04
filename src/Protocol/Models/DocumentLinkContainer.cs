using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentLinkContainer : Container<DocumentLink>
    {
        public DocumentLinkContainer() : this(Enumerable.Empty<DocumentLink>())
        {
        }

        public DocumentLinkContainer(IEnumerable<DocumentLink> items) : base(items)
        {
        }

        public DocumentLinkContainer(params DocumentLink[] items) : base(items)
        {
        }

        public static implicit operator DocumentLinkContainer(DocumentLink[] items) => new DocumentLinkContainer(items);

        public static implicit operator DocumentLinkContainer(Collection<DocumentLink> items) => new DocumentLinkContainer(items);

        public static implicit operator DocumentLinkContainer(List<DocumentLink> items) => new DocumentLinkContainer(items);
    }

    public class DocumentLinkContainer<T> : Container<DocumentLink<T>> where T : HandlerIdentity?, new()
    {
        public DocumentLinkContainer() : this(Enumerable.Empty<DocumentLink<T>>())
        {
        }

        public DocumentLinkContainer(IEnumerable<DocumentLink<T>> items) : base(items)
        {
        }

        public DocumentLinkContainer(params DocumentLink<T>[] items) : base(items)
        {
        }

        public static implicit operator DocumentLinkContainer<T>(DocumentLink<T>[] items) => new DocumentLinkContainer<T>(items);

        public static implicit operator DocumentLinkContainer<T>(Collection<DocumentLink<T>> items) => new DocumentLinkContainer<T>(items);

        public static implicit operator DocumentLinkContainer<T>(List<DocumentLink<T>> items) => new DocumentLinkContainer<T>(items);

        public static implicit operator DocumentLinkContainer(DocumentLinkContainer<T> container) => new DocumentLinkContainer(container.Select(z => (DocumentLink) z));
    }
}
