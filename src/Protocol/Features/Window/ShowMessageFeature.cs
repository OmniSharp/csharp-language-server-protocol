using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// The show message notification is sent from a server to a client to ask the client to display a particular message in the user interface.
        /// </summary>
        [Parallel]
        [Method(WindowNames.ShowMessage, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        public record ShowMessageParams : IRequest
        {
            /// <summary>
            /// The message type. See {@link MessageType}.
            /// </summary>
            public MessageType Type { get; init; }

            /// <summary>
            /// The actual message.
            /// </summary>
            public string Message { get; init; }
        }
    }

    namespace Window
    {
        public static partial class ShowMessageExtensions
        {
            public static void Show(this ILanguageServer mediator, ShowMessageParams @params) => mediator.SendNotification(@params);

            public static void ShowError(this ILanguageServer mediator, string message) => mediator.SendNotification(new ShowMessageParams { Type = MessageType.Error, Message = message });

            public static void Show(this ILanguageServer mediator, string message) => mediator.SendNotification(new ShowMessageParams { Type = MessageType.Log, Message = message });

            public static void ShowWarning(this ILanguageServer mediator, string message) =>
                mediator.SendNotification(new ShowMessageParams { Type = MessageType.Warning, Message = message });

            public static void ShowInfo(this ILanguageServer mediator, string message) => mediator.SendNotification(new ShowMessageParams { Type = MessageType.Info, Message = message });

            public static void Show(this IWindowLanguageServer mediator, ShowMessageParams @params) => mediator.SendNotification(@params);

            public static void ShowError(this IWindowLanguageServer mediator, string message) =>
                mediator.SendNotification(new ShowMessageParams { Type = MessageType.Error, Message = message });

            public static void Show(this IWindowLanguageServer mediator, string message) => mediator.SendNotification(new ShowMessageParams { Type = MessageType.Log, Message = message });

            public static void ShowWarning(this IWindowLanguageServer mediator, string message) =>
                mediator.SendNotification(new ShowMessageParams { Type = MessageType.Warning, Message = message });

            public static void ShowInfo(this IWindowLanguageServer mediator, string message) =>
                mediator.SendNotification(new ShowMessageParams { Type = MessageType.Info, Message = message });
        }
    }
}
