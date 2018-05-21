using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LspHandlerDescriptorDisposable : IDisposable
    {
        private readonly IEnumerable<ILspHandlerDescriptor> _instances;

        public LspHandlerDescriptorDisposable(IEnumerable<ILspHandlerDescriptor> instances)
        {
            _instances = instances;
        }

        public LspHandlerDescriptorDisposable(params ILspHandlerDescriptor[] instances)
        {
            _instances = instances;
        }

        public IEnumerable<ILspHandlerDescriptor> Descriptors => _instances;

        public void Dispose()
        {
            foreach (var instance in _instances)
                if (instance is IDisposable disposable)
                    disposable.Dispose();
        }
    }
}
