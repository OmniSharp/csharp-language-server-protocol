using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/willSave")]
    public interface IWillSaveTextDocumentHandler : INotificationHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions> { }
}