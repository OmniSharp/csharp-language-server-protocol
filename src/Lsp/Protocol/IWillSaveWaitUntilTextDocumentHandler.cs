using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/willSaveWaitUntil")]
    public interface IWillSaveWaitUntilTextDocumentHandler : IRequestHandler<WillSaveTextDocumentParams> { }
}