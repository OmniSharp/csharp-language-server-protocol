using System.Threading.Tasks;
using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    public static partial class ResponseHandlerExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this IClient mediator, ShowMessageRequestParams @params)
        {
            return mediator.SendRequest< ShowMessageRequestParams, MessageActionItem>("window/showMessageRequest", @params);
        }
    }
}