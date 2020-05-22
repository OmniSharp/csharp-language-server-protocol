using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.StepOut, Direction.ClientToServer)]
    public class StepOutArguments : IRequest<StepOutResponse>
    {
        /// <summary>
        /// Execute 'stepOut' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
