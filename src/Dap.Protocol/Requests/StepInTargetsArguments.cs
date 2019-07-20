using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StepInTargetsArguments : IRequest<StepInTargetsResponse>
    {
        /// <summary>
        /// The stack frame for which to retrieve the possible stepIn targets.
        /// </summary>
        public long frameId { get; set; }
    }

}
