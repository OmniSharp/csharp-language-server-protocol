using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static GeneralNames;
    [Serial, Method(Exit)]
    public interface IExitHandler : IJsonRpcNotificationHandler { }
}
