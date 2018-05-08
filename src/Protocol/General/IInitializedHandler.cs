using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static GeneralNames;
    public static partial class GeneralNames
    {
        public const string Initialized = "initialized";
    }

    [Serial, Method(Initialized)]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedParams> { }
}
