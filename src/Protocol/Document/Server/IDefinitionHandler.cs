using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.Definition)]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>, ICapability<DefinitionClientCapabilities> { }

    public abstract class DefinitionHandler : IDefinitionHandler
    {
        private readonly DefinitionRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public DefinitionHandler(DefinitionRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DefinitionRegistrationOptions GetRegistrationOptions() => _options;

        public Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            DefinitionParams request,
            IObserver<Container<LocationOrLocationLink>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DefinitionClientCapabilities capability) => Capability = capability;
        protected DefinitionClientCapabilities Capability { get; private set; }
    }

    public static class DefinitionHandlerExtensions
    {
        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DefinitionRegistrationOptions registrationOptions = null,
            Action<DefinitionClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new DefinitionRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DefinitionHandler
        {
            private readonly Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DefinitionClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<DefinitionClientCapabilities> setCapability,
                DefinitionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                DefinitionParams request,
                IObserver<Container<LocationOrLocationLink>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(DefinitionClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
