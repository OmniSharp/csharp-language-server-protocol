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
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, Container<DocumentHighlight>>, IRegistration<DocumentHighlightRegistrationOptions>, ICapability<DocumentHighlightClientCapabilities> { }

    public abstract class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        private readonly DocumentHighlightRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public DocumentHighlightHandler(DocumentHighlightRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DocumentHighlightRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<DocumentHighlight>> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<DocumentHighlight>> Handle(
            DocumentHighlightParams request,
            IObserver<Container<DocumentHighlight>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DocumentHighlightClientCapabilities capability) => Capability = capability;
        protected DocumentHighlightClientCapabilities Capability { get; private set; }
    }

    public static class DocumentHighlightHandlerExtensions
    {
        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> handler,
            DocumentHighlightRegistrationOptions registrationOptions = null,
            Action<DocumentHighlightClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentHighlightHandler
        {
            private readonly Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> _handler;
            private readonly Action<DocumentHighlightClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> handler,
                ProgressManager progressManager,
                Action<DocumentHighlightClientCapabilities> setCapability,
                DocumentHighlightRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<DocumentHighlight>> Handle(
                DocumentHighlightParams request,
                IObserver<Container<DocumentHighlight>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(DocumentHighlightClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
