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
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DefinitionCapability> { }

    public abstract class DefinitionHandler : IDefinitionHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DefinitionHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DefinitionCapability capability);
    }

    public static class DefinitionHandlerExtensions
    {
        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<DefinitionCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DefinitionHandler
        {
            private readonly Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                Action<DefinitionCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DefinitionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
