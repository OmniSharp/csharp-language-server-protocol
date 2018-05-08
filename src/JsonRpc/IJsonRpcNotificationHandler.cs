using System.Threading.Tasks;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcNotificationHandler : IRequestHandler<IRequest>, IJsonRpcHandler { }

    public interface IJsonRpcNotificationHandler<in TNotification> : IRequestHandler<TNotification>, IJsonRpcHandler
        where TNotification : IRequest
    { }
}
