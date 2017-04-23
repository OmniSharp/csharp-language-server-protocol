using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/didChangeWatchedFiles")]
    public interface IDidChangeWatchedFilesHandler : IRegistrableNotificationHandler<DidChangeWatchedFilesParams, object> { }
}