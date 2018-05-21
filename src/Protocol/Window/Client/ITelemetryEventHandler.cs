using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(WindowNames.TelemetryEvent)]
    public interface ITelemetryEventHandler : IJsonRpcNotificationHandler<TelemetryEventParams> { }
}
