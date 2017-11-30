using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class WindowNames
    {
        public const string LogMessage = "window/logMessage";
    }

    public static class LogMessageExtensions
    {
        public static void LogMessage(this IResponseRouter mediator, LogMessageParams @params)
        {
            mediator.SendNotification(WindowNames.LogMessage, @params);
        }

        public static void Log(this IResponseRouter mediator, LogMessageParams @params)
        {
            mediator.LogMessage(@params);
        }

        public static void LogError(this IResponseRouter mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this IResponseRouter mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this IResponseRouter mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this IResponseRouter mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
