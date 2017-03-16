using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/onTypeFormatting")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit> { }
}