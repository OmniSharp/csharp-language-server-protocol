using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentChangeRegistrationOptions))]
    [Method("textDocument/didChange")]
    public interface IDidChangeTextDocumentHandler : INotificationHandler<DidChangeTextDocumentParams> { }
}