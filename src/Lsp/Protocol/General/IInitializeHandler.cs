using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Method("initialize")]
    public interface IInitializeHandler : IRequestHandler<InitializeParams, InitializeResult> { }
}