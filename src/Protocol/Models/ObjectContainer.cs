using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ObjectContainer : ContainerBase<object>
    {
        public ObjectContainer() : this(Enumerable.Empty<object>())
        {
        }

        public ObjectContainer(IEnumerable<object> items) : base(items)
        {
        }

        public ObjectContainer(params object[] items) : base(items)
        {
        }

        public static implicit operator ObjectContainer(object[] items)
        {
            return new ObjectContainer(items);
        }

        public static implicit operator ObjectContainer(Collection<object> items)
        {
            return new ObjectContainer(items);
        }

        public static implicit operator ObjectContainer(List<object> items)
        {
            return new ObjectContainer(items);
        }
    }
}