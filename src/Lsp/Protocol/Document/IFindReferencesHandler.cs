using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/references")]
    public interface IFindReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer>, IRegistration<TextDocumentRegistrationOptions> { }
}