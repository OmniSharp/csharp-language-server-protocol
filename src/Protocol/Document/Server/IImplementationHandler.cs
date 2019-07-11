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
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ImplementationCapability> { }

    public abstract class ImplementationHandler : IImplementationHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public ImplementationHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(ImplementationParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(ImplementationCapability capability);
    }

    public static class ImplementationHandlerExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<ImplementationCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ImplementationHandler
        {
            private readonly Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<ImplementationCapability> _setCapability;

            public DelegatingHandler(
                Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                Action<ImplementationCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(ImplementationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ImplementationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
