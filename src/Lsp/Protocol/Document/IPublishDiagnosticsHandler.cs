using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ClientNotificationExtensions
    {
        public static Task PublishDiagnostics(this IClient mediator, PublishDiagnosticsParams @params)
        {
            return mediator.SendNotification("textDocument/publishDiagnostics", @params);
        }
    }
}