using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.Formatting)]
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, Container<TextEdit>>, IRegistration<DocumentFormattingRegistrationOptions>, ICapability<DocumentFormattingClientCapabilities> { }

    public abstract class DocumentFormattingHandler : IDocumentFormattingHandler
    {
        private readonly DocumentFormattingRegistrationOptions _options;
        public DocumentFormattingHandler(DocumentFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<TextEdit>> Handle(DocumentFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentFormattingClientCapabilities capability) => Capability = capability;
        protected DocumentFormattingClientCapabilities Capability { get; private set; }
    }

    public static class DocumentFormattingHandlerExtensions
    {
        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
            DocumentFormattingRegistrationOptions registrationOptions = null,
            Action<DocumentFormattingClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new DocumentFormattingRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentFormattingHandler
        {
            private readonly Func<DocumentFormattingParams, CancellationToken, Task<Container<TextEdit>>> _handler;
            private readonly Action<DocumentFormattingClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
                Action<DocumentFormattingClientCapabilities> setCapability,
                DocumentFormattingRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<TextEdit>> Handle(DocumentFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentFormattingClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
