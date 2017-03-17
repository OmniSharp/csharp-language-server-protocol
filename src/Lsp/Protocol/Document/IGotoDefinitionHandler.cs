using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/definition")]
    public interface IGotoDefinitionHandler : IRequestHandler<TextDocumentPositionParams, LocationOrLocations> { }
}