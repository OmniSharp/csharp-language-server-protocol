using JsonRPC;

namespace Lsp.Protocol
{
    [Method("exit")]
    public interface IExitHandler : INotificationHandler { }
}