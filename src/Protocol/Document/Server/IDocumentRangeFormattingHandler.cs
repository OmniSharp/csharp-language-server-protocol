using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.RangeFormatting)]
    public interface IDocumentRangeFormattingHandler : IJsonRpcRequestHandler<DocumentRangeFormattingParams, Container<TextEdit>>, IRegistration<DocumentRangeFormattingRegistrationOptions>, ICapability<DocumentRangeFormattingClientCapabilities> { }

    public abstract class DocumentRangeFormattingHandler : IDocumentRangeFormattingHandler
    {
        private readonly DocumentRangeFormattingRegistrationOptions _options;
        public DocumentRangeFormattingHandler(DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentRangeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<TextEdit>> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentRangeFormattingClientCapabilities capability) => Capability = capability;
        protected DocumentRangeFormattingClientCapabilities Capability { get; private set; }
    }

    public static class DocumentRangeFormattingHandlerExtensions
    {
        public static IDisposable OnDocumentRangeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentRangeFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
            DocumentRangeFormattingRegistrationOptions registrationOptions = null,
            Action<DocumentRangeFormattingClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new DocumentRangeFormattingRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentRangeFormattingHandler
        {
            private readonly Func<DocumentRangeFormattingParams, CancellationToken, Task<Container<TextEdit>>> _handler;
            private readonly Action<DocumentRangeFormattingClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentRangeFormattingParams, CancellationToken, Task<Container<TextEdit>>> handler,
                Action<DocumentRangeFormattingClientCapabilities> setCapability,
                DocumentRangeFormattingRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<TextEdit>> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentRangeFormattingClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
