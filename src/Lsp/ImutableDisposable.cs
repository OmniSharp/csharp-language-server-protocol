using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer
{
    class ImutableDisposable : IDisposable
    {
        private readonly IEnumerable<IDisposable> _instances;

        public ImutableDisposable(IEnumerable<IDisposable> instances)
        {
            _instances = instances;
        }

        public ImutableDisposable(params IDisposable[] instances)
        {
            _instances = instances;
        }

        public void Dispose()
        {
            foreach (var instance in _instances)
            {
                instance.Dispose();
            }
        }
    }
}