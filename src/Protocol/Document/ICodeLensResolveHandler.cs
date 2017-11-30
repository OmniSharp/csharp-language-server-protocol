using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static DocumentNames;
    public static partial class DocumentNames
    {
        public const string CodeLensResolve = "codeLens/resolve";
    }

    [Parallel, Method(CodeLensResolve)]
    public interface ICodeLensResolveHandler : IRequestHandler<CodeLens, CodeLens> { }
}
