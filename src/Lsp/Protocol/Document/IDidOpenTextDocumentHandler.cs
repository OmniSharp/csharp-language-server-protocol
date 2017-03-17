using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/didOpen")]
    public interface IDidOpenTextDocumentHandler : INotificationHandler<DidOpenTextDocumentParams> { }
}