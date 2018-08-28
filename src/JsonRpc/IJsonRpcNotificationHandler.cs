using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcNotificationHandler : IRequestHandler<EmptyRequest>, IJsonRpcHandler { }

    public interface IJsonRpcNotificationHandler<in TNotification> : IRequestHandler<TNotification>, IJsonRpcHandler
        where TNotification : IRequest
    { }
}
