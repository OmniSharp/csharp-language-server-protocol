using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Next, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface INextHandler : IJsonRpcRequestHandler<NextArguments, NextResponse>
    {
    }

    public abstract class NextHandler : INextHandler
    {
        public abstract Task<NextResponse> Handle(NextArguments request, CancellationToken cancellationToken);
    }
}
