using JsonRPC;
using Lsp.Models;

namespace Lsp.Protocol
{
    [Registration(typeof(ExecuteCommandRegistrationOptions))]
    [Method("workspace/executeCommand")]
    public interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams> { }
}