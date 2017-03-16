using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("window/showMessageRequest")]
    public interface IShowMessageRequestHandler : IRequestHandler<ShowMessageRequestParams, MessageActionItem> { }
}