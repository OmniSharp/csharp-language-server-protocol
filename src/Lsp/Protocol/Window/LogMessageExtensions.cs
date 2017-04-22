using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class LogMessageExtensions
    {
        public static Task LogMessage(this IOutgoingRequestRouter mediator, LogMessageParams @params)
        {
            return mediator.SendNotification("window/logMessage", @params);
        }
    }
}