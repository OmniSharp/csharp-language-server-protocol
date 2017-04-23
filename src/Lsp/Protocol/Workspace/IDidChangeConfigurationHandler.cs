using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/didChangeConfiguration")]
    public interface IDidChangeConfigurationHandler : INotificationHandler<DidChangeConfigurationParams>, IRegistration<object> { }
}