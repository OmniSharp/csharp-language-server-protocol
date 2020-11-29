using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    public abstract class ContainerBase<T> : IEnumerable<T>, IEquatable<ContainerBase<T>>
    {
        private readonly IEnumerable<T> _items;

        public ContainerBase(IEnumerable<T> items) => _items = items.ToArray();

        public override bool Equals(object? obj) => Equals(obj as ContainerBase<T>);

        public bool Equals(ContainerBase<T>? other) =>
            other is not null &&
            _items.SequenceEqual(other._items);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        public override int GetHashCode() => -566117206 + EqualityComparer<IEnumerable<T>>.Default.GetHashCode(_items);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(ContainerBase<T> base1, ContainerBase<T> base2) => EqualityComparer<ContainerBase<T>>.Default.Equals(base1, base2);

        public static bool operator !=(ContainerBase<T> base1, ContainerBase<T> base2) => !( base1 == base2 );
    }
}
