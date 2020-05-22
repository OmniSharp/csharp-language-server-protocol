using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcNotificationHandler : IRequestHandler<EmptyRequest>, IJsonRpcHandler { }

    public interface IJsonRpcNotificationHandler<in TNotification> : IRequestHandler<TNotification>, IJsonRpcHandler
        where TNotification : IRequest
    { }
}
