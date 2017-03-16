using JsonRPC;

namespace Lsp.Protocol
{
    [Method("telemetry/event")]
    public interface ITelemetryHandler : INotificationHandler<object> { }
}