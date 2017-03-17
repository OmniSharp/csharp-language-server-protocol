using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Registration(typeof(DocumentOnTypeFormattingRegistrationOptions))]
    [Method("textDocument/onTypeFormatting")]
    public interface IOnTypeFormatHandler : IRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer> { }
}