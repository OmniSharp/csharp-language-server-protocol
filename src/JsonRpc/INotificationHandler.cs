using System.Threading.Tasks;

namespace JsonRpc
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