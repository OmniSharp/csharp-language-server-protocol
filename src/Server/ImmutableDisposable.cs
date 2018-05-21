using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class ImmutableDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> _instances;

        public ImmutableDisposable(IEnumerable<IDisposable> instances)
        {
            _instances = instances;
        }

        public ImmutableDisposable(params IDisposable[] instances)
        {
            _instances = instances;
        }

        public void Dispose()
        {
            foreach (var instance in _instances)
                instance.Dispose();
        }
    }
}
