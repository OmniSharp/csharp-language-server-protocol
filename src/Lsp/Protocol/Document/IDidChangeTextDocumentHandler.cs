using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/didChange")]
    public interface IDidChangeTextDocumentHandler : INotificationHandler<DidChangeTextDocumentParams>, IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability> { }
}
