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
            mediator.SendNotification("window/showMessage", @params);
        }

        public static void LogError(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this ILanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
