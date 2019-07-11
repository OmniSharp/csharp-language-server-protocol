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
    public interface IReferencesHandler : IJsonRpcRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ReferencesCapability> { }

    public abstract class ReferencesHandler : IReferencesHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public ReferencesHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(ReferencesCapability capability);
    }

    public static class ReferencesHandlerExtensions
    {
        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, CancellationToken, Task<LocationContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<ReferencesCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ReferencesHandler
        {
            private readonly Func<ReferenceParams, CancellationToken, Task<LocationContainer>> _handler;
            private readonly Action<ReferencesCapability> _setCapability;

            public DelegatingHandler(
                Func<ReferenceParams, CancellationToken, Task<LocationContainer>> handler,
                Action<ReferencesCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ReferencesCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
