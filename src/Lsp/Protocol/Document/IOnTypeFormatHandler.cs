using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/onTypeFormatting")]
    public interface IOnTypeFormatHandler : IRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>, IRegistration<DocumentOnTypeFormattingRegistrationOptions> { }
}