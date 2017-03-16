using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("$/cancelRequest")]
    public interface ICancelRequestHandler : INotificationHandler<CancelParams> { }
}