using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("codeLens/resolve")]
    public interface ICodeLensResolveHandler : IRequestHandler<CodeLens, CodeLens> { }
}