using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.ShowMessage, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface IShowMessageHandler : IJsonRpcNotificationHandler<ShowMessageParams> { }

    public abstract class ShowMessageHandler : IShowMessageHandler
    {
        public abstract Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken);
    }

    public static partial class ShowMessageExtensions
    {
        public static void Show(this ILanguageServer mediator, ShowMessageParams @params)
        {
            mediator.ShowMessage(@params);
        }

        public static void ShowError(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Show(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void ShowWarning(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void ShowInfo(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Info, Message = message });
        }

        public static void Show(this IWindowLanguageServer mediator, ShowMessageParams @params)
        {
            mediator.ShowMessage(@params);
        }

        public static void ShowError(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Show(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void ShowWarning(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void ShowInfo(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
