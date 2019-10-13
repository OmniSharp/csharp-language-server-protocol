using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.TypeDefinition)]
    public interface ITypeDefinitionHandler : IJsonRpcRequestHandler<TypeDefinitionParams, LocationOrLocationLinks>, IRegistration<TypeDefinitionRegistrationOptions>, ICapability<TypeDefinitionCapability> { }

    public abstract class TypeDefinitionHandler : ITypeDefinitionHandler
    {
        private readonly TypeDefinitionRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public TypeDefinitionHandler(TypeDefinitionRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public TypeDefinitionRegistrationOptions GetRegistrationOptions() => _options;

        public Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, progressReporter, cancellationToken);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            TypeDefinitionParams request,
            IObserver<Container<LocationOrLocationLink>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(TypeDefinitionCapability capability) => Capability = capability;
        protected TypeDefinitionCapability Capability { get; private set; }
    }

    public static class TypeDefinitionHandlerExtensions
    {
        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions = null,
            Action<TypeDefinitionCapability> setCapability = null)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : TypeDefinitionHandler
        {
            private readonly Func<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<TypeDefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<TypeDefinitionCapability> setCapability,
                TypeDefinitionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                TypeDefinitionParams request,
                IObserver<Container<LocationOrLocationLink>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(TypeDefinitionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
