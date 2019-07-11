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
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentFormattingCapability> { }

    public abstract class DocumentFormattingHandler : IDocumentFormattingHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DocumentFormattingHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentFormattingParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DocumentFormattingCapability capability);
    }

    public static class DocumentFormattingHandlerExtensions
    {
        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<DocumentFormattingCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentFormattingHandler
        {
            private readonly Func<DocumentFormattingParams, CancellationToken, Task<TextEditContainer>> _handler;
            private readonly Action<DocumentFormattingCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
                Action<DocumentFormattingCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<TextEditContainer> Handle(DocumentFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentFormattingCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
