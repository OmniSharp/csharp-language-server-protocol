using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class LspHandlerDescriptorDisposable : IDisposable
    {
        private readonly IEnumerable<ILspHandlerDescriptor> _instances;
        private readonly IDisposable _disposable;

        public LspHandlerDescriptorDisposable(IEnumerable<ILspHandlerDescriptor> instances, IDisposable disposable)
        {
            _instances = instances;
            _disposable = disposable;
        }

        public IEnumerable<ILspHandlerDescriptor> Descriptors => _instances;

        public void Dispose()
        {
            _disposable.Dispose();
            foreach (var instance in _instances)
                if (instance is IDisposable disposable)
                    disposable.Dispose();
        }
    }
}
