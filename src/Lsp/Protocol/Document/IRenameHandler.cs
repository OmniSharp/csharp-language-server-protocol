using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/rename")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<TextDocumentRegistrationOptions> { }
}