using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>DisassembledInstruction
    /// Represents a single disassembled instruction.
    /// </summary>
    public class DisassembledInstruction
    {
        /// <summary>
        /// The address of the instruction. Treated as a hex value if prefixed with '0x', or as a decimal value otherwise.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Optional raw bytes representing the instruction and its operands, in an implementation-defined format.
        /// </summary>
        [Optional] public string InstructionBytes { get; set; }

        /// <summary>
        /// Text representing the instruction and its operands, in an implementation-defined format.
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Name of the symbol that correponds with the location of this instruction, if any.
        /// </summary>
        [Optional] public string Symbol { get; set; }

        /// <summary>
        /// Source location that corresponds to this instruction, if any. Should always be set (if available) on the first instruction returned, but can be omitted afterwards if this instruction maps to the same source file as the previous instruction.
        /// </summary>
        [Optional] public Source Location { get; set; }

        /// <summary>
        /// The line within the source location that corresponds to this instruction, if any.
        /// </summary>
        [Optional] public long? Line { get; set; }

        /// <summary>
        /// The column within the line that corresponds to this instruction, if any.
        /// </summary>
        [Optional] public long? Column { get; set; }

        /// <summary>
        /// The end line of the range that corresponds to this instruction, if any.
        /// </summary>
        [Optional] public long? EndLine { get; set; }

        /// <summary>
        /// The end column of the range that corresponds to this instruction, if any.
        /// </summary>
        [Optional] public long? EndColumn { get; set; }
    }
}