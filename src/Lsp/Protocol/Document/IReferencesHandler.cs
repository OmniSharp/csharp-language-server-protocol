using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/references")]
    public interface IReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<ReferencesCapability> { }
}