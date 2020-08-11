using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Variables, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IVariablesHandler : IJsonRpcRequestHandler<VariablesArguments, VariablesResponse>
    {
    }

    public abstract class VariablesHandler : IVariablesHandler
    {
        public abstract Task<VariablesResponse> Handle(VariablesArguments request, CancellationToken cancellationToken);
    }
}
