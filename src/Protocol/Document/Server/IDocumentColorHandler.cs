using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.DocumentColor)]
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class DocumentColorHandler : IDocumentColorHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public DocumentColorHandler(DocumentColorRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<ColorPresentation>> Handle(DocumentColorParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, progressReporter, cancellationToken);
        }

        public abstract Task<Container<ColorPresentation>> Handle(
            DocumentColorParams request,
            IObserver<Container<ColorPresentation>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(ColorProviderCapability capability) => Capability = capability;
        protected ColorProviderCapability Capability { get; private set; }
    }

    public static class DocumentColorHandlerExtensions
    {
        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions = null,
            Action<ColorProviderCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentColorHandler
        {
            private readonly Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<ColorPresentation>>> _handler;
            private readonly Action<ColorProviderCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, WorkDoneProgressReporter, CancellationToken, Task<Container<ColorPresentation>>> handler,
                ProgressManager progressManager,
                Action<ColorProviderCapability> setCapability,
                DocumentColorRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<ColorPresentation>> Handle(
                DocumentColorParams request,
                IObserver<Container<ColorPresentation>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(ColorProviderCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
