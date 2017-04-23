using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/definition")]
    public interface IGotoDefinitionHandler : IRegistrableRequestHandler<TextDocumentPositionParams, LocationOrLocations, TextDocumentRegistrationOptions> { }
}