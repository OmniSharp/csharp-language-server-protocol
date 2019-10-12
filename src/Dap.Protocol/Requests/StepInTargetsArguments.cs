using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StepInTargetsArguments : IRequest<StepInTargetsResponse>
    {
        /// <summary>
        /// The stack frame for which to retrieve the possible stepIn targets.
        /// </summary>
        public long FrameId { get; set; }
    }

}
