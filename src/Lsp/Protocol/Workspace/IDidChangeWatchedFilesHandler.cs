using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/didChangeWatchedFiles")]
    public interface IDidChangeWatchedFilesHandler : INotificationHandler<DidChangeWatchedFilesParams>, IRegistration<object>, ICapability<DidChangeWatchedFilesCapability> { }
}