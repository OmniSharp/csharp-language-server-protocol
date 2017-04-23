using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/executeCommand")]
    public interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions> { }
}