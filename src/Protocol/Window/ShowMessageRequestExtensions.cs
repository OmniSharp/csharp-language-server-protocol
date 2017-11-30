using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class WindowNames
    {
        public const string ShowMessageRequest = "window/showMessageRequest";
    }

    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this IResponseRouter mediator, ShowMessageRequestParams @params)
        {
            return mediator.SendRequest<ShowMessageRequestParams, MessageActionItem>(WindowNames.ShowMessageRequest, @params);
        }

        public static Task<MessageActionItem> Show(this IResponseRouter mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }

        public static Task<MessageActionItem> Request(this IResponseRouter mediator, ShowMessageRequestParams @params)
        {
            return mediator.ShowMessage(@params);
        }
    }
}
