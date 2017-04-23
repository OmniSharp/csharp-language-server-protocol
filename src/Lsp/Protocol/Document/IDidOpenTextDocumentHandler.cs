using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didOpen")]
    public interface IDidOpenTextDocumentHandler : IRegistrableNotificationHandler<DidOpenTextDocumentParams, TextDocumentRegistrationOptions> { }
}