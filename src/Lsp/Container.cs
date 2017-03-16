using System.Collections.Generic;
using System.Linq;

namespace Lsp
{
    public class Container<T> : ContainerBase<T>
    {
        public Container() : this(Enumerable.Empty<T>())
        {
        }

        public Container(IEnumerable<T> items) : base(items)
        {
        }

        public Container(params T[] items) : base(items)
        {
        }

        public static implicit operator Container<T>(T[] items)
        {
            return new Container<T>(items);
        }
    }
}