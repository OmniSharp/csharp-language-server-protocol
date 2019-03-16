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
    public interface ITypeDefinitionHandler : IJsonRpcRequestHandler<TypeDefinitionParams, LocationOrLocationLinks>, IRegistration<TextDocumentRegistrationOptions>, ICapability<TypeDefinitionCapability> { }

    public abstract class TypeDefinitionHandler : ITypeDefinitionHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public TypeDefinitionHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(TypeDefinitionCapability capability);
    }

    public static class TypeDefinitionHandlerExtensions
    {
        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<TypeDefinitionCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : TypeDefinitionHandler
        {
            private readonly Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<TypeDefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                Action<TypeDefinitionCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(TypeDefinitionCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
