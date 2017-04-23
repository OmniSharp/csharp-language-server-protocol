using System.Collections.Generic;
using System.Linq;

namespace Lsp
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
    }
}