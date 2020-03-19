using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(WindowNames.LogMessage)]
    public interface ILogMessageHandler : IJsonRpcNotificationHandler<LogMessageParams> { }
}
