using JsonRpc;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("exit")]
    public interface IExitHandler : INotificationHandler { }
}