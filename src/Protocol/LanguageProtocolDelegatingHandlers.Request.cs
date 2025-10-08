using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol;

public static partial class LanguageProtocolDelegatingHandlers
{
    public sealed class Request<TParams, TResult, TRegistrationOptions, TCapability> :
        AbstractHandlers.Base<TRegistrationOptions, TCapability>,
        IJsonRpcRequestHandler<TParams, TResult>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<TResult>
        where TRegistrationOptions : class, new()
        where TCapability : ICapability
    {
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public Request(
            Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
        }

        public Request(
            Func<TParams, TCapability, CancellationToken, Task<TResult>> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
        ) :
            this(Guid.Empty, handler, registrationOptionsFactory)
        {
        }

        Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) =>
            _handler(request, Capability, cancellationToken);

        protected internal override TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(capability, clientCapabilities);
    }

    public sealed class Request<TParams, TRegistrationOptions, TCapability> :
        AbstractHandlers.Base<TRegistrationOptions, TCapability>,
        IJsonRpcRequestHandler<TParams>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<Unit>
        where TRegistrationOptions : class, new()
        where TCapability : ICapability
    {
        private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions, TCapability> _registrationOptionsFactory;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public Request(
            Guid id, Func<TParams, TCapability, CancellationToken, Task> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
        }

        public Request(
            Func<TParams, TCapability, CancellationToken, Task> handler,
            RegistrationOptionsDelegate<TRegistrationOptions, TCapability> registrationOptionsFactory
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

    public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
        AbstractHandlers.Base<TRegistrationOptions>,
        IJsonRpcRequestHandler<TParams, TResult>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<TResult>
        where TRegistrationOptions : class, new()
    {
        private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public RequestRegistration(
            Func<TParams, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory
        ) :
            this(Guid.Empty, handler, registrationOptionsFactory)
        {
        }

        public RequestRegistration(
            Guid id, Func<TParams, CancellationToken, Task<TResult>> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
        }

        Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request, CancellationToken cancellationToken) =>
            _handler(request, cancellationToken);


        protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(clientCapabilities);
    }

    public sealed class RequestRegistration<TParams, TRegistrationOptions> :
        AbstractHandlers.Base<TRegistrationOptions>,
        IJsonRpcRequestHandler<TParams>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<Unit>
        where TRegistrationOptions : class, new()
    {
        private readonly Func<TParams, CancellationToken, Task> _handler;
        private readonly RegistrationOptionsDelegate<TRegistrationOptions> _registrationOptionsFactory;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public RequestRegistration(
            Guid id, Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory
        )
        {
            _id = id;
            _handler = handler;
            _registrationOptionsFactory = registrationOptionsFactory;
        }

        public RequestRegistration(
            Func<TParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TRegistrationOptions> registrationOptionsFactory
        ) :
            this(Guid.Empty, handler, registrationOptionsFactory)
        {
        }

        async Task<Unit> IRequestHandler<TParams, Unit>.
            Handle(TParams request, CancellationToken cancellationToken)
        {
            await _handler(request, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }

        protected override TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) =>
            _registrationOptionsFactory(clientCapabilities);
    }

    public sealed class RequestCapability<TParams, TResult, TCapability> :
        AbstractHandlers.BaseCapability<TCapability>,
        IJsonRpcRequestHandler<TParams, TResult>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<TResult>
        where TCapability : ICapability
    {
        private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public RequestCapability(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler) :
            this(Guid.Empty, handler)
        {
        }

        public RequestCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task<TResult>> handler)
        {
            _id = id;
            _handler = handler;
        }

        Task<TResult> IRequestHandler<TParams, TResult>.
            Handle(TParams request, CancellationToken cancellationToken) =>
            _handler(request, Capability, cancellationToken);
    }

    public sealed class RequestCapability<TParams, TCapability> :
        AbstractHandlers.BaseCapability<TCapability>,
        IJsonRpcRequestHandler<TParams>,
        ICanBeIdentifiedHandler
        where TParams : IRequest<Unit>
        where TCapability : ICapability
    {
        private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
        private readonly Guid _id;
        Guid ICanBeIdentifiedHandler.Id => _id;

        public RequestCapability(Func<TParams, TCapability, CancellationToken, Task> handler) :
            this(Guid.Empty, handler)
        {
        }

        public RequestCapability(Guid id, Func<TParams, TCapability, CancellationToken, Task> handler)
        {
            _id = id;
            _handler = handler;
        }

        async Task<Unit> IRequestHandler<TParams, Unit>.
            Handle(TParams request, CancellationToken cancellationToken)
        {
            await _handler(request, Capability, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
