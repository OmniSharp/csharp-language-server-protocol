using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.DocumentLink)]
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, DocumentLinkContainer>, IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> { }

    [Parallel, Method(DocumentNames.DocumentLinkResolve)]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink> { }

    public abstract class DocumentLinkHandler : IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        private readonly DocumentLinkRegistrationOptions _options;
        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentLinkRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken);
        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public abstract bool CanResolve(DocumentLink value);
        public abstract void SetCapability(DocumentLinkCapability capability);
    }

    public static class DocumentLinkHandlerExtensions
    {
        public static IDisposable OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler = null,
            Func<DocumentLink, bool> canResolve = null,
            DocumentLinkRegistrationOptions registrationOptions = null,
            Action<DocumentLinkCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentLinkHandler
        {
            private readonly Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> _handler;
            private readonly Func<DocumentLink, CancellationToken, Task<DocumentLink>> _resolveHandler;
            private readonly Func<DocumentLink, bool> _canResolve;
            private readonly Action<DocumentLinkCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
                Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
                Func<DocumentLink, bool> canResolve,
                Action<DocumentLinkCapability> setCapability,
                DocumentLinkRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(DocumentLink value) => _canResolve.Invoke(value);
            public override void SetCapability(DocumentLinkCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
