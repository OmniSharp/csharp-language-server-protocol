using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(TextDocumentRegistrationOptions))]
    [Method("textDocument/definition")]
    public interface IGotoDefinitionHandler : IRequestHandler<TextDocumentPositionParams, LocationOrLocations> { }
}