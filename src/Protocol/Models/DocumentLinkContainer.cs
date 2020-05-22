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

        public static implicit operator DocumentLinkContainer(DocumentLink[] items)
        {
            return new DocumentLinkContainer(items);
        }

        public static implicit operator DocumentLinkContainer(Collection<DocumentLink> items)
        {
            return new DocumentLinkContainer(items);
        }

        public static implicit operator DocumentLinkContainer(List<DocumentLink> items)
        {
            return new DocumentLinkContainer(items);
        }
    }
}
