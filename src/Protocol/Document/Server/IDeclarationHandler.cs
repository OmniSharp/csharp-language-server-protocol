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
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DeclarationCapability> { }

    public abstract class DeclarationHandler : IDeclarationHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DeclarationHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DeclarationCapability capability);
    }

    public static class DeclarationHandlerExtensions
    {
        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<DeclarationCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DeclarationHandler
        {
            private readonly Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DeclarationCapability> _setCapability;

            public DelegatingHandler(
                Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                Action<DeclarationCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DeclarationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
