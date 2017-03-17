using System.Threading.Tasks;

namespace JsonRpc
{
    public interface INotificationHandler<TNotification>
    {
        Task Handle(TNotification notification);
    }
}