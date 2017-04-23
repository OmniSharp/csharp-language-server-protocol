using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/rename")]
    public interface IRenameHandler : IRegistrableRequestHandler<RenameParams, WorkspaceEdit, TextDocumentRegistrationOptions> { }
}