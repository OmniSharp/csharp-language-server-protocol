using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/didClose")]
    public interface IDidCloseTextDocumentHandler : INotificationHandler<DidCloseTextDocumentParams> { }
}