using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("codeLens/resolve")]
    public interface ICodeLensResolveHandler : IRequestHandler<CodeLens, CodeLens> { }
}