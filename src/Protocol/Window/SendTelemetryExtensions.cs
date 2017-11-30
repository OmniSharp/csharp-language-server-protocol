using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this IResponseRouter mediator, object @params)
        {
            mediator.SendNotification("telemetry/event", @params);
        }
    }
}
