using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.Implementation)]
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>, IRegistration<ImplementationRegistrationOptions>, ICapability<ImplementationCapability> { }

    public abstract class ImplementationHandler : IImplementationHandler
    {
        private readonly ImplementationRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public ImplementationHandler(ImplementationRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public ImplementationRegistrationOptions GetRegistrationOptions() => _options;

        public async Task<LocationOrLocationLinks> Handle(ImplementationParams request, CancellationToken cancellationToken)
        {
            using var partialResults = _progressManager.For(request, cancellationToken);
            using var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return await Handle(request, partialResults, progressReporter, cancellationToken).ConfigureAwait(false);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            ImplementationParams request,
            IObserver<Container<LocationOrLocationLink>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(ImplementationCapability capability) => Capability = capability;
        protected ImplementationCapability Capability { get; private set; }
    }

    public static class ImplementationHandlerExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions = null,
            Action<ImplementationCapability> setCapability = null)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : ImplementationHandler
        {
            private readonly Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<ImplementationCapability> _setCapability;

            public DelegatingHandler(
                Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<ImplementationCapability> setCapability,
                ImplementationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                ImplementationParams request,
                IObserver<Container<LocationOrLocationLink>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(ImplementationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
