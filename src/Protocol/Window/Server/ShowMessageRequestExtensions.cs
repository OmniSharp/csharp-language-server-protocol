using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this ILanguageServerWindow mediator, ShowMessageRequestParams @params)
        {
            return mediator.SendRequest<ShowMessageRequestParams, MessageActionItem>(WindowNames.ShowMessageRequest, @params);
        }

        public static Task<MessageActionItem> Show(this ILanguageServerWindow mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }

        public static Task<MessageActionItem> Request(this ILanguageServerWindow mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }
    }
}
