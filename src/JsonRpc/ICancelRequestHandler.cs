using System.Threading;
using System.Threading.Tasks;
using MediatR;


namespace OmniSharp.Extensions.JsonRpc
{
    [Parallel, Method(JsonRpcNames.CancelRequest, Direction.Bidirectional)]
    public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

    public abstract class CancelRequestHandler : ICancelRequestHandler
    {
        public abstract Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken);
    }
}
