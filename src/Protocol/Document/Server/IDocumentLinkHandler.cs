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
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, Container<DocumentLink>>, IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> { }

    [Parallel, Method(DocumentNames.DocumentLinkResolve)]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink> { }

    public abstract class DocumentLinkHandler : IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        private readonly DocumentLinkRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentLinkRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<DocumentLink>> Handle(DocumentLinkParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<DocumentLink>> Handle(
            DocumentLinkParams request,
            IObserver<Container<DocumentLink>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public abstract bool CanResolve(DocumentLink value);
        public virtual void SetCapability(DocumentLinkCapability capability) => Capability = capability;
        protected DocumentLinkCapability Capability { get; private set; }
    }

    public static class DocumentLinkHandlerExtensions
    {
        public static IDisposable OnDocumentLink(
            this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, IObserver<Container<DocumentLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentLink>>> handler,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler = null,
            Func<DocumentLink, bool> canResolve = null,
            DocumentLinkRegistrationOptions registrationOptions = null,
            Action<DocumentLinkCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, registry.ProgressManager, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentLinkHandler
        {
            private readonly Func<DocumentLinkParams, IObserver<Container<DocumentLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentLink>>> _handler;
            private readonly Func<DocumentLink, CancellationToken, Task<DocumentLink>> _resolveHandler;
            private readonly Func<DocumentLink, bool> _canResolve;
            private readonly Action<DocumentLinkCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentLinkParams, IObserver<Container<DocumentLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentLink>>> handler,
                Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
                ProgressManager progressManager,
                Func<DocumentLink, bool> canResolve,
                Action<DocumentLinkCapability> setCapability,
                DocumentLinkRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<Container<DocumentLink>> Handle(
                DocumentLinkParams request,
                IObserver<Container<DocumentLink>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(DocumentLink value) => _canResolve.Invoke(value);
            public override void SetCapability(DocumentLinkCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
