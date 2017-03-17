using System.Threading.Tasks;
using JsonRPC;

namespace Lsp.Protocol
{
    public static partial class ClientNotificationExtensions
    {
        public static Task SendTelemetry(this IClient mediator, object @params)
        {
            return mediator.SendNotification("telemetry/event", @params);
        }
    }
}