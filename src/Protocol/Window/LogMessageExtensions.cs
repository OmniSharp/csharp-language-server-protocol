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

        public static void Log(this ILanguageServer mediator, LogMessageParams @params)
        {
            mediator.LogMessage(@params);
        }

        public static void LogError(this ILanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this ILanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this ILanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this ILanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
