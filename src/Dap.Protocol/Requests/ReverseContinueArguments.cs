using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.ReverseContinue, Direction.ClientToServer)]
    public class ReverseContinueArguments : IRequest<ReverseContinueResponse>
    {
        /// <summary>
        /// Execute 'reverseContinue' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
