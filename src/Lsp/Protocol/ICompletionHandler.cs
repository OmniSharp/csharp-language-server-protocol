using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(CompletionRegistrationOptions))]
    [Method("textDocument/completion")]
    public interface ICompletionHandler : IRequestHandler<TextDocumentPositionParams, CompletionList> { }
}