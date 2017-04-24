using System.Threading.Tasks;
using JsonRpc;

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