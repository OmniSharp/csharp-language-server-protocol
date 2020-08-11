using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel]
    [Method(WindowNames.LogMessage, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface ILogMessageHandler : IJsonRpcNotificationHandler<LogMessageParams>
    {
    }

    public abstract class LogMessageHandler : ILogMessageHandler
    {
        public abstract Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken);
    }

    public static partial class LogMessageExtensions
    {
        public static void Log(this ILanguageServer mediator, LogMessageParams @params) => mediator.LogMessage(@params);

        public static void LogError(this ILanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Error, Message = message });

        public static void Log(this ILanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Log, Message = message });

        public static void LogWarning(this ILanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Warning, Message = message });

        public static void LogInfo(this ILanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Info, Message = message });

        public static void Log(this IWindowLanguageServer mediator, LogMessageParams @params) => mediator.LogMessage(@params);

        public static void LogError(this IWindowLanguageServer mediator, string message) =>
            mediator.LogMessage(new LogMessageParams { Type = MessageType.Error, Message = message });

        public static void Log(this IWindowLanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Log, Message = message });

        public static void LogWarning(this IWindowLanguageServer mediator, string message) =>
            mediator.LogMessage(new LogMessageParams { Type = MessageType.Warning, Message = message });

        public static void LogInfo(this IWindowLanguageServer mediator, string message) => mediator.LogMessage(new LogMessageParams { Type = MessageType.Info, Message = message });
    }
}
