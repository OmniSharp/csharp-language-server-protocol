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
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>, IRegistration<ImplementationRegistrationOptions>, ICapability<ImplementationClientCapabilities> { }

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

        public Task<LocationOrLocationLinks> Handle(ImplementationParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            ImplementationParams request,
            IObserver<Container<LocationOrLocationLink>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(ImplementationClientCapabilities capability) => Capability = capability;
        protected ImplementationClientCapabilities Capability { get; private set; }
    }

    public static class ImplementationHandlerExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions = null,
            Action<ImplementationClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : ImplementationHandler
        {
            private readonly Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<ImplementationClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<ImplementationClientCapabilities> setCapability,
                ImplementationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                ImplementationParams request,
                IObserver<Container<LocationOrLocationLink>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(ImplementationClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
