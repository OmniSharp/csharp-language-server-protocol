using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// A GotoTarget describes a code location that can be used as a target in the ‘goto’ request.
    /// The possible goto targets can be determined via the ‘gotoTargets’ request.
    /// </summary>
    public class GotoTarget
    {
        /// <summary>
        /// Unique identifier for a goto target. This is used in the goto request.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the goto target (shown in the UI).
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The line of the goto target.
        /// </summary>
        public long Line { get; set; }

        /// <summary>
        /// An optional column of the goto target.
        /// </summary>
        [Optional] public long? Column { get; set; }

        /// <summary>
        /// An optional end line of the range covered by the goto target.
        /// </summary>
        [Optional] public long? EndLine { get; set; }

        /// <summary>
        /// An optional end column of the range covered by the goto target.
        /// </summary>
        [Optional] public long? EndColumn { get; set; }

        /// <summary>
        /// Optional memory reference for the instruction pointer value represented by this target.
        /// </summary>
        [Optional] public string InstructionPointerReference { get; set; }
    }
}