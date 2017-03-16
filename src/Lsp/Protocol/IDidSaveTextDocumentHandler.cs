using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentSaveRegistrationOptions))]
    [Method("textDocument/didSave")]
    public interface IDidSaveTextDocumentHandler : INotificationHandler<DidSaveTextDocumentParams> { }
}