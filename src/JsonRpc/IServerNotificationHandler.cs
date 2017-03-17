using System.Threading.Tasks;

namespace JsonRpc
{
    public interface INotificationHandler
    {
        Task Handle();
    }
}