using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/hover")]
    public interface IHoverHandler : IRegistrableRequestHandler<TextDocumentPositionParams, Hover, TextDocumentRegistrationOptions> { }
}