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
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, DocumentHighlightContainer>, IRegistration<DocumentHighlightRegistrationOptions>, ICapability<DocumentHighlightCapability> { }

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

        public async Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            using var partialResults = _progressManager.For(request, cancellationToken);
            using var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return await Handle(request, partialResults, progressReporter, cancellationToken).ConfigureAwait(false);
        }

        public abstract Task<DocumentHighlightContainer> Handle(
            DocumentHighlightParams request,
            IObserver<DocumentHighlightContainer> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DocumentHighlightCapability capability) => Capability = capability;
        protected DocumentHighlightCapability Capability { get; private set; }
    }

    public static class DocumentHighlightHandlerExtensions
    {
        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, IObserver<DocumentHighlightContainer>, WorkDoneProgressReporter, CancellationToken, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions = null,
            Action<DocumentHighlightCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentHighlightHandler
        {
            private readonly Func<DocumentHighlightParams, IObserver<DocumentHighlightContainer>, WorkDoneProgressReporter, CancellationToken, Task<DocumentHighlightContainer>> _handler;
            private readonly Action<DocumentHighlightCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentHighlightParams, IObserver<DocumentHighlightContainer>, WorkDoneProgressReporter, CancellationToken, Task<DocumentHighlightContainer>> handler,
                ProgressManager progressManager,
                Action<DocumentHighlightCapability> setCapability,
                DocumentHighlightRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<DocumentHighlightContainer> Handle(
                DocumentHighlightParams request,
                IObserver<DocumentHighlightContainer> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(DocumentHighlightCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
