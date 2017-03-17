using System.Threading.Tasks;

namespace JsonRPC
{
    public interface INotificationHandler<TNotification>
    {
        Task Handle(TNotification notification);
    }
}