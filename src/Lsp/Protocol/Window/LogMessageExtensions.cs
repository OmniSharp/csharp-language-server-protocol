using OmniSharp.Extensions.LanguageServerProtocol;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class LogMessageExtensions
    {
        public static void LogMessage(this ILanguageServer mediator, LogMessageParams @params)
        {
            mediator.SendNotification("window/logMessage", @params);
        }
    }
}