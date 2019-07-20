namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StepInTargetsResponse
    {
        /// <summary>
        /// The possible stepIn targets of the specified source location.
        /// </summary>
        public Container<StepInTarget> Targets { get; set; }
    }

}
