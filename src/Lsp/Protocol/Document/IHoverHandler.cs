using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/hover")]
    public interface IHoverHandler : IRequestHandler<TextDocumentPositionParams, Hover>, IRegistration<TextDocumentRegistrationOptions>, ICapability<HoverCapability> { }
}