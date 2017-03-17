using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Method("initialize")]
    public interface IInitializeHandler : IRequestHandler<InitializeParams, InitializeResult> { }
}