using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/executeCommand")]
    public interface IExecuteCommandHandler : IRegistrableRequestHandler<ExecuteCommandParams, ExecuteCommandRegistrationOptions> { }
}