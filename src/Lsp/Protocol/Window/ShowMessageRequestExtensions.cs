using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this ILanguageServer mediator, ShowMessageRequestParams @params)
        {
            return mediator.SendRequest<ShowMessageRequestParams, MessageActionItem>("window/showMessageRequest", @params);
        }

        public static Task<MessageActionItem> Show(this ILanguageServer mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }

        public static Task<MessageActionItem> Request(this ILanguageServer mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }
    }
}
