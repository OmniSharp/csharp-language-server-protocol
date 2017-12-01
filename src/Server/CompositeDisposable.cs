using System;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _instances;

        public CompositeDisposable(IEnumerable<IDisposable> instances)
        {
            _instances = instances.ToList();
        }

        public CompositeDisposable(params IDisposable[] instances)
        {
            _instances = instances.ToList();
        }

        public void Add(params IDisposable[] disposable)
        {
            _instances.AddRange(disposable);
        }

        public void Remove(IDisposable disposable)
        {
            _instances.Remove(disposable);
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
