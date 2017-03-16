using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("textDocument/publishDiagnostics")]
    public interface IPublishDiagnosticsHandler : INotificationHandler<PublishDiagnosticsParams> { }
}