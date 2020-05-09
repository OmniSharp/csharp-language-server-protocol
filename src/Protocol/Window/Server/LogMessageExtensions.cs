using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class LogMessageExtensions
    {
        public static void LogMessage(this ILanguageServerWindow mediator, LogMessageParams @params)
        {
            mediator.SendNotification(WindowNames.LogMessage, @params);
        }

        public static void Log(this ILanguageServerWindow mediator, LogMessageParams @params)
        {
            mediator.LogMessage(@params);
        }

        public static void LogError(this ILanguageServerWindow mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this ILanguageServerWindow mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this ILanguageServerWindow mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this ILanguageServerWindow mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
