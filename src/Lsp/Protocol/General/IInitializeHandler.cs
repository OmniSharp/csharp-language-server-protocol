using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Method("initialize")]
    public interface IInitializeHandler : IRequestHandler<InitializeParams, InitializeResult> { }
}