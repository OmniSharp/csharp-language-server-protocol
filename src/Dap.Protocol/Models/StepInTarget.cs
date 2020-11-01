namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A StepInTarget can be used in the ‘stepIn’ request and determines into which single target the stepIn request should step.
    /// </summary>
    public class StepInTarget
    {
        /// <summary>
        /// Unique identifier for a stepIn target.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the stepIn target (shown in the UI).
        /// </summary>
        public string Label { get; set; } = null!;
    }
}
