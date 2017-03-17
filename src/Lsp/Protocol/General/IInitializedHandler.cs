using JsonRPC;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("initialized")]
    public interface IInitializedHandler : INotificationHandler { }
}