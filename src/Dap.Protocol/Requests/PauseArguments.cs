using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Pause, Direction.ClientToServer)]
    public class PauseArguments : IRequest<PauseResponse>
    {
        /// <summary>
        /// Pause execution for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
