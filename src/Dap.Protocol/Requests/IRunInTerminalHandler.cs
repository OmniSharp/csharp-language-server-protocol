using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.RunInTerminal, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IRunInTerminalHandler : IJsonRpcRequestHandler<RunInTerminalArguments, RunInTerminalResponse>
    {
    }

    public abstract class RunInTerminalHandler : IRunInTerminalHandler
    {
        public abstract Task<RunInTerminalResponse> Handle(RunInTerminalArguments request, CancellationToken cancellationToken);
    }
}
