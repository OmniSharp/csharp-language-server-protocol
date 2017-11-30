using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method("initialize")]
    public interface IInitializeHandler : IRequestHandler<InitializeParams, InitializeResult> { }
}
