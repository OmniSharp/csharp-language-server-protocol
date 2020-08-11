using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.SetVariable, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ISetVariableHandler : IJsonRpcRequestHandler<SetVariableArguments, SetVariableResponse>
    {
    }

    public abstract class SetVariableHandler : ISetVariableHandler
    {
        public abstract Task<SetVariableResponse> Handle(
            SetVariableArguments request,
            CancellationToken cancellationToken
        );
    }
}
