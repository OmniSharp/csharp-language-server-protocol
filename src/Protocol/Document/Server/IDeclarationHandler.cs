using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.Declaration)]
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<DeclarationRegistrationOptions>, ICapability<DeclarationCapability> { }

    public abstract class DeclarationHandler : IDeclarationHandler
    {
        private readonly DeclarationRegistrationOptions _options;
        protected ProgressManager ProgressManager { get; }
        public DeclarationHandler(DeclarationRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DeclarationRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DeclarationCapability capability) => Capability = capability;
        protected DeclarationCapability Capability { get; private set; }
    }

    public static class DeclarationHandlerExtensions
    {
        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions = null,
            Action<DeclarationCapability> setCapability = null)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DeclarationHandler
        {
            private readonly Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DeclarationCapability> _setCapability;

            public DelegatingHandler(
                Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<DeclarationCapability> setCapability,
                DeclarationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DeclarationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
