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
        private readonly ProgressManager _progressManager;
        public DeclarationHandler(DeclarationRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DeclarationRegistrationOptions GetRegistrationOptions() => _options;

        public async Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken)
        {
            using var partialResults = _progressManager.For(request, cancellationToken);
            using var progressReporter = _progressManager.Delegate(request, cancellationToken);
            return await Handle(request, partialResults, progressReporter, cancellationToken).ConfigureAwait(false);
        }

        public abstract Task<LocationOrLocationLinks> Handle(
            DeclarationParams request,
            IObserver<Container<LocationLink>> partialResults,
            WorkDoneProgressReporter progressReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DeclarationCapability capability) => Capability = capability;
        protected DeclarationCapability Capability { get; private set; }
    }

    public static class DeclarationHandlerExtensions
    {
        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, IObserver<Container<LocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions = null,
            Action<DeclarationCapability> setCapability = null)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DeclarationHandler
        {
            private readonly Func<DeclarationParams, IObserver<Container<LocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DeclarationCapability> _setCapability;

            public DelegatingHandler(
                Func<DeclarationParams, IObserver<Container<LocationLink>>, WorkDoneProgressReporter, CancellationToken, Task<LocationOrLocationLinks>> handler,
                ProgressManager progressManager,
                Action<DeclarationCapability> setCapability,
                DeclarationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(
                DeclarationParams request,
                IObserver<Container<LocationLink>> partialResults,
                WorkDoneProgressReporter progressReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, progressReporter, cancellationToken);
            public override void SetCapability(DeclarationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
