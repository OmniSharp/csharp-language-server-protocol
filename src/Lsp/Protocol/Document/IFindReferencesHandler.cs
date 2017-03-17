using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/references")]
    public interface IFindReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer> { }
}