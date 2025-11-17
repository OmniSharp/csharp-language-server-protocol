using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class AbstractHandlers
    {
        public abstract class Notification<TParams> : IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Notification<TParams, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
            where TRegistrationOptions : class, new()
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class NotificationCapability<TParams, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams>
            where TParams : IRequest<Unit>
            where TCapability : ICapability
        {
            public abstract Task<Unit> Handle(TParams request, CancellationToken cancellationToken);
        }
    }
}
