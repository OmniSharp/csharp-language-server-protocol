using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
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

        public static implicit operator Container<T>(Collection<T> items)
        {
            return new Container<T>(items);
        }

        public static implicit operator Container<T>(List<T> items)
        {
            return new Container<T>(items);
        }
    }
}
