using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class LogMessageExtensions
    {
        public static void LogMessage(this ILanguageServer mediator, LogMessageParams @params)
        {
            mediator.SendNotification("window/logMessage", @params);
        }
    }
}