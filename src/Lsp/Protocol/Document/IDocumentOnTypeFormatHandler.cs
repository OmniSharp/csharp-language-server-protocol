using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/onTypeFormatting")]
    public interface IDocumentOnTypeFormatHandler : IRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>, IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingCapability> { }
}