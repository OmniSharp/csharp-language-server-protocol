using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/rename")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit> { }
}