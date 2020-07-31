using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

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

        /// <summary>
        /// Convert from a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public DocumentLinkContainer<T> Convert<T>(ISerializer serializer) where T : class
        {
            return new DocumentLinkContainer<T>(this.Select(z => z.From<T>(serializer)));
        }
    }
    public class DocumentLinkContainer<T> : Container<DocumentLink<T>> where T : class
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

        public static implicit operator DocumentLinkContainer<T>(DocumentLink<T>[] items)
        {
            return new DocumentLinkContainer<T>(items);
        }

        public static implicit operator DocumentLinkContainer<T>(Collection<DocumentLink<T>> items)
        {
            return new DocumentLinkContainer<T>(items);
        }

        public static implicit operator DocumentLinkContainer<T>(List<DocumentLink<T>> items)
        {
            return new DocumentLinkContainer<T>(items);
        }

        /// <summary>
        /// Convert to a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal DocumentLinkContainer Convert(ISerializer serializer)
        {
            return new DocumentLinkContainer(this.Select(z => z.To(serializer)));
        }
    }
}
