using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/willSave")]
    public interface IWillSaveTextDocumentHandler : INotificationHandler<WillSaveTextDocumentParams> { }
}