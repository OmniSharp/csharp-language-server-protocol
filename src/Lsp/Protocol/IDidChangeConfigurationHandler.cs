using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(object))]
    [Method("workspace/didChangeConfiguration")]
    public interface IDidChangeConfigurationHandler : INotificationHandler<DidChangeConfigurationParams> { }
}