using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Disassemble, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record DisassembleArguments : IRequest<DisassembleResponse>
        {
            /// <summary>
            /// Memory reference to the base location containing the instructions to disassemble.
            /// </summary>
            public string MemoryReference { get; init; } = null!;

            /// <summary>
            /// Optional offset(in bytes) to be applied to the reference location before disassembling.Can be negative.
            /// </summary>
            [Optional]
            public long? Offset { get; init; }

            /// <summary>
            /// Optional offset(in instructions) to be applied after the byte offset(if any) before disassembling.Can be negative.
            /// </summary>

            [Optional]
            public long? InstructionOffset { get; init; }

            /// <summary>
            /// Number of instructions to disassemble starting at the specified location and offset.An adapter must return exactly this number of instructions - any
            /// unavailable instructions
            /// should be replaced with an implementation-defined 'invalid instruction' value.
            /// </summary>
            public long InstructionCount { get; init; }

            /// <summary>
            /// If true, the adapter should attempt to resolve memory addresses and other values to symbolic names.
            /// </summary>
            [Optional]
            public bool ResolveSymbols { get; init; }
        }

        public record DisassembleResponse
        {
            /// <summary>
            /// The list of disassembled instructions.
            /// </summary>
            public Container<DisassembledInstruction> Instructions { get; init; } = null!;
        }
    }

    namespace Models
    {
        /// <summary>
        /// DisassembledInstruction
        /// Represents a single disassembled instruction.
        /// </summary>
        public record DisassembledInstruction
        {
            /// <summary>
            /// The address of the instruction. Treated as a hex value if prefixed with '0x', or as a decimal value otherwise.
            /// </summary>
            public string Address { get; init; } = null!;

            /// <summary>
            /// Optional raw bytes representing the instruction and its operands, in an implementation-defined format.
            /// </summary>
            [Optional]
            public string? InstructionBytes { get; init; }

            /// <summary>
            /// Text representing the instruction and its operands, in an implementation-defined format.
            /// </summary>
            public string Instruction { get; init; } = null!;

            /// <summary>
            /// Name of the symbol that corresponds with the location of this instruction, if any.
            /// </summary>
            [Optional]
            public string? Symbol { get; init; }

            /// <summary>
            /// Source location that corresponds to this instruction, if any. Should always be set (if available) on the first instruction returned, but can be omitted
            /// afterwards if this
            /// instruction maps to the same source file as the previous instruction.
            /// </summary>
            [Optional]
            public Source? Location { get; init; }

            /// <summary>
            /// The line within the source location that corresponds to this instruction, if any.
            /// </summary>
            [Optional]
            public int? Line { get; init; }

            /// <summary>
            /// The column within the line that corresponds to this instruction, if any.
            /// </summary>
            [Optional]
            public int? Column { get; init; }

            /// <summary>
            /// The end line of the range that corresponds to this instruction, if any.
            /// </summary>
            [Optional]
            public int? EndLine { get; init; }

            /// <summary>
            /// The end column of the range that corresponds to this instruction, if any.
            /// </summary>
            [Optional]
            public int? EndColumn { get; init; }
        }
    }
}
