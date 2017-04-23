using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("textDocument/rangeFormatting")]
    public interface IDocumentRangeFormattingHandler : IRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }
}