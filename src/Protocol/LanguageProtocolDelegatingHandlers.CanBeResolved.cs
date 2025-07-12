using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class LanguageProtocolDelegatingHandlers
    {
        public sealed class CanBeResolved<TItem, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, TCapability, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(
                Guid id, Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler,
                RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
            )
            {
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
                _id = id;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, Capability, cancellationToken);

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
                _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>,
            ICanBeIdentifiedHandler
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public CanBeResolved(Guid id, Func<TItem, CancellationToken, Task<TItem>> resolveHandler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _resolveHandler = resolveHandler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.Handle(TItem request, CancellationToken cancellationToken) => _resolveHandler(request, cancellationToken);

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
        }
    }
}
