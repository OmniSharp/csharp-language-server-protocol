using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/willSave")]
    public interface IWillSaveTextDocumentHandler : IRegistrableNotificationHandler<WillSaveTextDocumentParams, TextDocumentRegistrationOptions> { }
}