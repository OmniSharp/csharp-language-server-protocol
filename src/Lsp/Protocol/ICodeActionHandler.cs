using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/codeAction")]
    public interface ICodeActionHandler : IRequestHandler<CodeActionParams, CommandContainer> { }
}