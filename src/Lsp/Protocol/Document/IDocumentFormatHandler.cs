using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/formatting")]
    public interface IDocumentFormatHandler : IRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions> { }
}