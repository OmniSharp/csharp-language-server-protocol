using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.LogMessage, Direction.ServerToClient)]
    public interface ILogMessageHandler : IJsonRpcNotificationHandler<LogMessageParams> { }

    public abstract class LogMessageHandler : ILogMessageHandler
    {
        public abstract Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken);
    }

    public static class LogMessageExtensions
    {
        public static IDisposable OnLogMessage(
            this ILanguageClientRegistry registry,
            Action<LogMessageParams> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnLogMessage(
            this ILanguageClientRegistry registry,
            Action<LogMessageParams, CancellationToken> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnLogMessage(
            this ILanguageClientRegistry registry,
            Func<LogMessageParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnLogMessage(
            this ILanguageClientRegistry registry,
            Func<LogMessageParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }

        public static void LogMessage(this IWindowLanguageServer mediator, LogMessageParams @params)
        {
            mediator.SendNotification(@params);
        }

        public static void Log(this IWindowLanguageServer mediator, LogMessageParams @params)
        {
            mediator.LogMessage(@params);
        }

        public static void LogError(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
