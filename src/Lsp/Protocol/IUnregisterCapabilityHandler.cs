using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("client/unregisterCapability")]
    public interface IUnregisterCapabilityHandler : IRequestHandler<UnregistrationParams, object> { }
}