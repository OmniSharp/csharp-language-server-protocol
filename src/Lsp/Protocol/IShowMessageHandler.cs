using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("window/showMessage")]
    public interface IShowMessageHandler : INotificationHandler<ShowMessageParams> { }
}