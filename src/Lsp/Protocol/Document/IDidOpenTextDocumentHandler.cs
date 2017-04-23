using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didOpen")]
    public interface IDidOpenTextDocumentHandler : INotificationHandler<DidOpenTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }
}