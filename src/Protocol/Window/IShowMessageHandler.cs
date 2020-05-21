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
    [Parallel, Method(WindowNames.ShowMessage, Direction.ServerToClient)]
    public interface IShowMessageHandler : IJsonRpcNotificationHandler<ShowMessageParams> { }

    public abstract class ShowMessageHandler : IShowMessageHandler
    {
        public abstract Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken);
    }

    public static class ShowMessageExtensions
    {
        public static IDisposable OnShowMessage(
            this ILanguageClientRegistry registry,
            Action<ShowMessageParams> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnShowMessage(
            this ILanguageClientRegistry registry,
            Action<ShowMessageParams, CancellationToken> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnShowMessage(
            this ILanguageClientRegistry registry,
            Func<ShowMessageParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnShowMessage(
            this ILanguageClientRegistry registry,
            Func<ShowMessageParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }

        public static void ShowMessage(this IWindowLanguageServer mediator, ShowMessageParams @params)
        {
            mediator.SendNotification(@params);
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
