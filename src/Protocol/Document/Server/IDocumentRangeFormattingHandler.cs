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
    public interface IDocumentRangeFormattingHandler : IJsonRpcRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }

    public abstract class DocumentRangeFormattingHandler : IDocumentRangeFormattingHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DocumentRangeFormattingHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DocumentRangeFormattingCapability capability);
    }

    public static class DocumentRangeFormattingHandlerExtensions
    {
        public static IDisposable OnDocumentRangeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentRangeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<DocumentRangeFormattingCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentRangeFormattingHandler
        {
            private readonly Func<DocumentRangeFormattingParams, CancellationToken, Task<TextEditContainer>> _handler;
            private readonly Action<DocumentRangeFormattingCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentRangeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
                Action<DocumentRangeFormattingCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<TextEditContainer> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentRangeFormattingCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
