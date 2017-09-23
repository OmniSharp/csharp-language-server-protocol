using OmniSharp.Extensions.LanguageServer;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this ILanguageServer mediator, object @params)
        {
            mediator.SendNotification("telemetry/event", @params);
        }
    }
}