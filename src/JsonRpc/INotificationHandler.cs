using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface INotificationHandler : IJsonRpcHandler
    {
        Task Handle();
    }

    public interface INotificationHandler<TNotification> : IJsonRpcHandler
    {
        Task Handle(TNotification notification);
    }
}