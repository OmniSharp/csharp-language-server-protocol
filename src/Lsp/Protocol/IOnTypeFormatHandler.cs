using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(DocumentOnTypeFormattingRegistrationOptions))]
    [Method("textDocument/onTypeFormatting")]
    public interface IOnTypeFormatHandler : IRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer> { }
}