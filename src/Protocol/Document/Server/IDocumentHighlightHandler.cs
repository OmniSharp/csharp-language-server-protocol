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
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, Container<DocumentHighlight>>, IRegistration<DocumentHighlightRegistrationOptions>, ICapability<DocumentHighlightCapability> { }

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

        public virtual void SetCapability(DocumentHighlightCapability capability) => Capability = capability;
        protected DocumentHighlightCapability Capability { get; private set; }
    }

    public static class DocumentHighlightHandlerExtensions
    {
        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> handler,
            DocumentHighlightRegistrationOptions registrationOptions = null,
            Action<DocumentHighlightCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentHighlightHandler
        {
            private readonly Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> _handler;
            private readonly Action<DocumentHighlightCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<DocumentHighlight>>> handler,
                ProgressManager progressManager,
                Action<DocumentHighlightCapability> setCapability,
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
            public override void SetCapability(DocumentHighlightCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
