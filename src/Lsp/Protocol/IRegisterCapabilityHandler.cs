using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Method("client/registerCapability")]
    public interface IRegisterCapabilityHandler : IRequestHandler<RegistrationParams, object> { }
}