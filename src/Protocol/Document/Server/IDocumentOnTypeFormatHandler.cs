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
    public interface IDocumentOnTypeFormatHandler : IJsonRpcRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>, IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingCapability> { }

    public abstract class DocumentOnTypeFormatHandler : IDocumentOnTypeFormatHandler
    {
        private readonly DocumentOnTypeFormattingRegistrationOptions _options;
        public DocumentOnTypeFormatHandler(DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DocumentOnTypeFormattingCapability capability);
    }

    public static class DocumentOnTypeFormatHandlerExtensions
    {
        public static IDisposable OnDocumentOnTypeFormat(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions = null,
            Action<DocumentOnTypeFormattingCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentOnTypeFormatHandler
        {
            private readonly Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> _handler;
            private readonly Action<DocumentOnTypeFormattingCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
                Action<DocumentOnTypeFormattingCapability> setCapability,
                DocumentOnTypeFormattingRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentOnTypeFormattingCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
