using JsonRPC;

namespace Lsp.Protocol
{
    [Method("initialized")]
    public interface IInitializedHandler : INotificationHandler { }
}