using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/documentLink")]
    public interface IDocumentLinkHandler : IRequestHandler<DocumentLinkParams, DocumentLink>, IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability> { }
}