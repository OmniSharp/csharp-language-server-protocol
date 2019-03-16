using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.DocumentHighlight)]
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, DocumentHighlightContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentHighlightCapability> { }

    public abstract class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DocumentHighlightHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DocumentHighlightCapability capability);
    }

    public static class DocumentHighlightHandlerExtensions
    {
        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<DocumentHighlightCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentHighlightHandler
        {
            private readonly Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> _handler;
            private readonly Action<DocumentHighlightCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
                Action<DocumentHighlightCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentHighlightCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
