using JsonRPC;

namespace Lsp.Protocol
{
    [Method("shutdown")]
    public interface IShutdownHandler : INotificationHandler { }
}