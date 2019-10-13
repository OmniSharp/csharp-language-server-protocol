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
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderClientCapabilities> { }

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
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<ColorPresentation>> Handle(
            DocumentColorParams request,
            IObserver<Container<ColorPresentation>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(ColorProviderClientCapabilities capability) => Capability = capability;
        protected ColorProviderClientCapabilities Capability { get; private set; }
    }

    public static class DocumentColorHandlerExtensions
    {
        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions = null,
            Action<ColorProviderClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentColorHandler
        {
            private readonly Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<ColorPresentation>>> _handler;
            private readonly Action<ColorProviderClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentColorParams, IObserver<Container<ColorPresentation>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<ColorPresentation>>> handler,
                ProgressManager progressManager,
                Action<ColorProviderClientCapabilities> setCapability,
                DocumentColorRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<ColorPresentation>> Handle(
                DocumentColorParams request,
                IObserver<Container<ColorPresentation>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(ColorProviderClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
