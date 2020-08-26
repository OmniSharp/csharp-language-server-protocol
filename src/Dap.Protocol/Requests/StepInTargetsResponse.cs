using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StepInTargetsResponse
    {
        /// <summary>
        /// The possible stepIn targets of the specified source location.
        /// </summary>
        public Container<StepInTarget>? Targets { get; set; }
    }
}
