using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/onTypeFormatting")]
    public interface IOnTypeFormatHandler : IRegistrableRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer, DocumentOnTypeFormattingRegistrationOptions> { }
}