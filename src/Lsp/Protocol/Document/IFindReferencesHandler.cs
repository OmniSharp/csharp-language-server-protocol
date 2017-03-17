using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/references")]
    public interface IFindReferencesHandler : IRequestHandler<ReferenceParams, LocationContainer> { }
}