using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/codeLens")]
    public interface ICodeLensHandler : IRequestHandler<CodeLensParams, CodeLensContainer>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensCapability> { }
}