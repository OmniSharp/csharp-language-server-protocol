using System.Threading.Tasks;

namespace JsonRPC
{
    public interface INotificationHandler
    {
        Task Handle();
    }
}