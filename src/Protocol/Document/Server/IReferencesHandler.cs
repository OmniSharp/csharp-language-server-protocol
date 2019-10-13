using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.References)]
    public interface IReferencesHandler : IJsonRpcRequestHandler<ReferenceParams, Container<Location>>, IRegistration<ReferenceRegistrationOptions>, ICapability<ReferenceClientCapabilities> { }

    public abstract class ReferencesHandler : IReferencesHandler
    {
        private readonly ReferenceRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public ReferencesHandler(ReferenceRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public ReferenceRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<Location>> Handle(ReferenceParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<Location>> Handle(
            ReferenceParams request,
            IObserver<Container<Container<Location>>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(ReferenceClientCapabilities capability) => Capability = capability;
        protected ReferenceClientCapabilities Capability { get; private set; }
    }

    public static class ReferencesHandlerExtensions
    {
        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, IObserver<Container<Container<Location>>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<Location>>> handler,
            ReferenceRegistrationOptions registrationOptions = null,
            Action<ReferenceClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : ReferencesHandler
        {
            private readonly Func<ReferenceParams, IObserver<Container<Container<Location>>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<Location>>> _handler;
            private readonly Action<ReferenceClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<ReferenceParams, IObserver<Container<Container<Location>>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<Location>>> handler,
                ProgressManager progressManager,
                Action<ReferenceClientCapabilities> setCapability,
                ReferenceRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<Location>> Handle(
                ReferenceParams request,
                IObserver<Container<Container<Location>>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);

            public override void SetCapability(ReferenceClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
