using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WindowNames.LogMessage, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        public record LogMessageParams : IRequest
        {
            /// <summary>
            /// The message type. See {@link MessageType}
            /// </summary>
            public MessageType Type { get; init; }

            /// <summary>
            /// The actual message
            /// </summary>
            public string Message { get; init; } = null!;
        }
    }

    namespace Window
    {
        public static partial class LogMessageExtensions
        {
            public static void Log(this ILanguageServer mediator, LogMessageParams @params)
            {
                mediator.SendNotification(@params);
            }

            public static void LogError(this ILanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Error, Message = message });
            }

            public static void Log(this ILanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Log, Message = message });
            }

            public static void LogWarning(this ILanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Warning, Message = message });
            }

            public static void LogInfo(this ILanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Info, Message = message });
            }

            public static void Log(this IWindowLanguageServer mediator, LogMessageParams @params)
            {
                mediator.SendNotification(@params);
            }

            public static void LogError(this IWindowLanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Error, Message = message });
            }

            public static void Log(this IWindowLanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Log, Message = message });
            }

            public static void LogWarning(this IWindowLanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Warning, Message = message });
            }

            public static void LogInfo(this IWindowLanguageServer mediator, string message)
            {
                mediator.SendNotification(new LogMessageParams { Type = MessageType.Info, Message = message });
            }
        }
    }
}
