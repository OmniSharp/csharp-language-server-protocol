using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class ContainerBase<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;

        public ContainerBase(IEnumerable<T> items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
