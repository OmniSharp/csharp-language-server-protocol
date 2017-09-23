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
    }
}