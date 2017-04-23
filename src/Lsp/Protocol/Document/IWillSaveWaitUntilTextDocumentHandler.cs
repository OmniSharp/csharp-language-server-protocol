using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/willSaveWaitUntil")]
    public interface IWillSaveWaitUntilTextDocumentHandler : IRequestHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions> { }
}