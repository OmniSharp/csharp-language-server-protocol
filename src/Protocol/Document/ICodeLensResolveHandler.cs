using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method("codeLens/resolve")]
    public interface ICodeLensResolveHandler : IRequestHandler<CodeLens, CodeLens> { }
}
