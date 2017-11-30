using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static WindowNames;
    public static partial class WindowNames
    {
        public const string TelemetryEvent = "telemetry/event";
    }

    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this IResponseRouter mediator, object @params)
        {
            mediator.SendNotification(TelemetryEvent, @params);
        }
    }
}
