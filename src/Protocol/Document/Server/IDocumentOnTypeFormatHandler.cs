using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.OnTypeFormatting)]
    public interface IDocumentOnTypeFormatHandler : IJsonRpcRequestHandler<DocumentOnTypeFormattingParams, Container<TextEdit>>, IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingClientCapabilities> { }

    public abstract class DocumentOnTypeFormatHandler : IDocumentOnTypeFormatHandler
    {
        private readonly DocumentOnTypeFormattingRegistrationOptions _options;
        public DocumentOnTypeFormatHandler(DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<TextEdit>> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentOnTypeFormattingClientCapabilities capability) => Capability = capability;
        protected DocumentOnTypeFormattingClientCapabilities Capability { get; private set; }
    }

    public static class DocumentOnTypeFormatHandlerExtensions
    {
        public static IDisposable OnDocumentOnTypeFormat(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions = null,
            Action<DocumentOnTypeFormattingClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentOnTypeFormatHandler
        {
            private readonly Func<DocumentOnTypeFormattingParams, CancellationToken, Task<Container<TextEdit>>> _handler;
            private readonly Action<DocumentOnTypeFormattingClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentOnTypeFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
                Action<DocumentOnTypeFormattingClientCapabilities> setCapability,
                DocumentOnTypeFormattingRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<TextEdit>> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentOnTypeFormattingClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
