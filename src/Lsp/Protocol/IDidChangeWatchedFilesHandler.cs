using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(object))]
    [Method("workspace/didChangeWatchedFiles")]
    public interface IDidChangeWatchedFilesHandler : INotificationHandler<DidChangeWatchedFilesParams> { }
}