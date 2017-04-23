using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didClose")]
    public interface IDidCloseTextDocumentHandler : INotificationHandler<DidCloseTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions> { }
}