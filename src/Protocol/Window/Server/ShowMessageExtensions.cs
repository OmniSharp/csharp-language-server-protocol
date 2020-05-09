using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class ShowMessageExtensions
    {
        public static void ShowMessage(this ILanguageServerWindow mediator, ShowMessageParams @params)
        {
            mediator.SendNotification(WindowNames.ShowMessage, @params);
        }

        public static void Show(this ILanguageServerWindow mediator, ShowMessageParams @params)
        {
            mediator.ShowMessage(@params);
        }

        public static void ShowError(this ILanguageServerWindow mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Show(this ILanguageServerWindow mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void ShowWarning(this ILanguageServerWindow mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void ShowInfo(this ILanguageServerWindow mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
