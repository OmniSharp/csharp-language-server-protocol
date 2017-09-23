using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServerProtocol;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this ILanguageServer mediator, ShowMessageRequestParams @params)
        {
            return mediator.SendRequest<ShowMessageRequestParams, MessageActionItem>("window/showMessageRequest", @params);
        }
    }
}