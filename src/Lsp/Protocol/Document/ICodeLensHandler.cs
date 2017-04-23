using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("textDocument/codeLens")]
    public interface ICodeLensHandler : IRegistrableRequestHandler<CodeLensParams, CodeLensContainer, CodeLensRegistrationOptions> { }
}