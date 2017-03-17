using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class LogMessageExtensions
    {
        public static Task LogMessage(this IClient mediator, LogMessageParams @params)
        {
            return mediator.SendNotification("window/logMessage", @params);
        }
    }
}