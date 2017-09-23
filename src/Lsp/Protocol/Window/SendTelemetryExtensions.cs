using OmniSharp.Extensions.LanguageServerProtocol;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this ILanguageServer mediator, object @params)
        {
            mediator.SendNotification("telemetry/event", @params);
        }
    }
}