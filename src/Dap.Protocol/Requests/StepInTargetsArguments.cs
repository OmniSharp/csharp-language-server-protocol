using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.StepInTargets, Direction.ClientToServer)]
    public class StepInTargetsArguments : IRequest<StepInTargetsResponse>
    {
        /// <summary>
        /// The stack frame for which to retrieve the possible stepIn targets.
        /// </summary>
        public long FrameId { get; set; }
    }
}
