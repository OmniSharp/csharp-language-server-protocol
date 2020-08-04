using System.Collections;
using System.Collections.Generic;

namespace OmniSharp.Extensions.JsonRpc
{
    class JsonRpcHandlerCollection : IJsonRpcHandlerCollection
    {
        private readonly List<JsonRpcHandlerDescription> _descriptions = new List<JsonRpcHandlerDescription>();
        public IEnumerator<JsonRpcHandlerDescription> GetEnumerator() => _descriptions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IJsonRpcHandlerCollection Add(JsonRpcHandlerDescription description)
        {
            _descriptions.Add(description);
            return this;
        }
    }
}
