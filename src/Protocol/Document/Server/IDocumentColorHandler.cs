using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.DocumentColor)]
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorInformation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class DocumentColorHandler : IDocumentColorHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        public DocumentColorHandler(DocumentColorRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorInformation>> Handle(DocumentColorParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(ColorProviderCapability capability);
    }

    public static class DocumentColorHandlerExtensions
    {
        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, CancellationToken, Task<Container<ColorInformation>>> handler,
            DocumentColorRegistrationOptions registrationOptions = null,
            Action<ColorProviderCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentColorHandler
        {
            private readonly Func<DocumentColorParams, CancellationToken, Task<Container<ColorInformation>>> _handler;
            private readonly Action<ColorProviderCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentColorParams, CancellationToken, Task<Container<ColorInformation>>> handler,
                Action<ColorProviderCapability> setCapability,
                DocumentColorRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<ColorInformation>> Handle(DocumentColorParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ColorProviderCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
