using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("window/logMessage")]
    public interface ILogMessageHandler : INotificationHandler<LogMessageParams> { }
}