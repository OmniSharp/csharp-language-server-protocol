using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class LanguageProtocolDelegatingHandlers
    {
        public sealed class Notification<TParams, TRegistrationOptions, TCapability> :
            AbstractHandlers.Base<TRegistrationOptions, TCapability>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<Unit>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(
                Guid id, Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
            )
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Notification(
                Func<TParams, TCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
            ) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
                _registrationOptionsFactory(capability, clientCapabilities);
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            AbstractHandlers.Base<TRegistrationOptions>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<Unit>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public Notification(Guid id, Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory)
            {
                _id = id;
                _handler = handler;
                _registrationOptionsFactory = registrationOptionsFactory;
            }

            public Notification(Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory) :
                this(Guid.Empty, handler, registrationOptionsFactory)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => _registrationOptionsFactory(clientCapabilities);
        }

        public sealed class NotificationCapability<TParams, TCapability> :
            AbstractHandlers.BaseCapability<TCapability>,
            IJsonRpcNotificationHandler<TParams>,
            ICanBeIdentifiedHandler
            where TParams : IRequest<Unit>
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly Guid _id;
            Guid ICanBeIdentifiedHandler.Id => _id;

            public NotificationCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _id = id;
                _handler = handler;
            }

            public NotificationCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
                this(Guid.Empty, handler)
            {
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }
    }
}
