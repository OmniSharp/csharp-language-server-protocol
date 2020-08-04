using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class ValueOptionsFactory<T> : IOptionsFactory<T> where T : JsonRpcServerOptions, new() {
        private readonly T _options;

        public ValueOptionsFactory(T options)
        {
            _options = options;
        }

        public T Create(string name) => _options;
    }
}