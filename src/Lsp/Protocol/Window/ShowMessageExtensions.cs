using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class ShowMessageExtensions
    {
        public static void ShowMessage(this ILanguageServer mediator, ShowMessageParams @params)
        {
            mediator.SendNotification("window/showMessage", @params);
        }

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
    }
}
