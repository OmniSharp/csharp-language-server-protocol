using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/onTypeFormatting")]
    public interface IRenameHandler : IRequestHandler<RenameParams, WorkspaceEdit> { }
}