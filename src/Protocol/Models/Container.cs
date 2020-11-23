using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class Container<T> : ContainerBase<T>
    {
        public Container() : this(Enumerable.Empty<T>()) { }

        public Container(IEnumerable<T> items) : base(items) { }

        public Container(params T[] items) : base(items) { }

        public static Container<T> Create(IEnumerable<T> items) => new(items);
        public static implicit operator Container<T>(T[] items) => new(items);
        public static Container<T> Create(params T[] items) => new(items);
        public static implicit operator Container<T>(Collection<T> items) => new(items);
        public static Container<T> Create(Collection<T> items) => new(items);
        public static implicit operator Container<T>(List<T> items) => new(items);
        public static Container<T> Create(List<T> items) => new(items);
        public static implicit operator Container<T>(in ImmutableArray<T> items) => new(items);
        public static Container<T> Create(in ImmutableArray<T> items) => new(items);
        public static implicit operator Container<T>(ImmutableList<T> items) => new(items);
        public static Container<T> Create(ImmutableList<T> items) => new(items);
    }
}
