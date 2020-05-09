

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this ILanguageServerWindow mediator, object @params)
        {
            mediator.SendNotification(WindowNames.TelemetryEvent, @params);
        }
    }
}
