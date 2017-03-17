using JsonRpc;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("shutdown")]
    public interface IShutdownHandler : INotificationHandler { }
}