using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(CodeLensRegistrationOptions))]
    [Method("textDocument/codeLens")]
    public interface ICodeLensHandler : IRequestHandler<CodeLensParams, CodeLensContainer> { }
}