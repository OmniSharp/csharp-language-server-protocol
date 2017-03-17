using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ClientNotificationExtensions
    {
        public static Task LogMessage(this IClient mediator, LogMessageParams @params)
        {
            return mediator.SendNotification("window/logMessage", @params);
        }
    }
}