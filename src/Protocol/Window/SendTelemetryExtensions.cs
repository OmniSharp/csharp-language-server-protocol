using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class WindowNames
    {
        public const string TelemetryEvent = "telemetry/event";
    }

    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this IResponseRouter mediator, object @params)
        {
            mediator.SendNotification(WindowNames.TelemetryEvent, @params);
        }
    }
}
