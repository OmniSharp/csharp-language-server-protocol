using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class LspHandlerDescriptorDisposable : IDisposable
    {
        private readonly IDisposable _disposable;

        public LspHandlerDescriptorDisposable(IEnumerable<ILspHandlerDescriptor> instances, IDisposable disposable)
        {
            Descriptors = instances;
            _disposable = disposable;
        }

        public IEnumerable<ILspHandlerDescriptor> Descriptors { get; }

        public void Dispose()
        {
            _disposable.Dispose();
            foreach (var instance in Descriptors)
                if (instance is IDisposable disposable)
                    disposable.Dispose();
        }
    }
}
