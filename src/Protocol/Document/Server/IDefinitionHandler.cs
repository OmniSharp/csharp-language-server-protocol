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
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>, ICapability<DefinitionCapability> { }

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
            var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, progressReporter, cancellationToken);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            DefinitionParams request,
            IObserver<Container<LocationOrLocationLink>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DefinitionCapability capability) => Capability = capability;
        protected DefinitionCapability Capability { get; private set; }
    }

    public static class DefinitionHandlerExtensions
    {
        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DefinitionRegistrationOptions registrationOptions = null,
            Action<DefinitionCapability> setCapability = null)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DefinitionHandler
        {
            private readonly Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<DefinitionCapability> setCapability,
                DefinitionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                DefinitionParams request,
                IObserver<Container<LocationOrLocationLink>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(DefinitionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
