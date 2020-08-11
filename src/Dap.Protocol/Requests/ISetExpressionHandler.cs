using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.SetExpression, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ISetExpressionHandler : IJsonRpcRequestHandler<SetExpressionArguments, SetExpressionResponse>
    {
    }

    public abstract class SetExpressionHandler : ISetExpressionHandler
    {
        public abstract Task<SetExpressionResponse> Handle(
            SetExpressionArguments request,
            CancellationToken cancellationToken
        );
    }
}
