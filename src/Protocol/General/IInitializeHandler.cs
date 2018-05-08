using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static GeneralNames;
    public static partial class GeneralNames
    {
        public const string Initialize = "initialize";
    }

    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method(Initialize)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> { }
}
