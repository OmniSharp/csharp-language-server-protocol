using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.StepBack, Direction.ClientToServer)]
    public class StepBackArguments : IRequest<StepBackResponse>
    {
        /// <summary>
        /// Execute 'stepBack' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
