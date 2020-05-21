using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Disassemble, Direction.ClientToServer)]
    public class DisassembleArguments : IRequest<DisassembleResponse>
    {
        /// <summary>
        /// Memory reference to the base location containing the instructions to disassemble.
        /// </summary>
        public string MemoryReference { get; set; }

        /// <summary>
        /// Optional offset(in bytes) to be applied to the reference location before disassembling.Can be negative.
        /// </summary>
        [Optional] public long? Offset { get; set; }

        /// <summary>
        /// Optional offset(in instructions) to be applied after the byte offset(if any) before disassembling.Can be negative.
        /// </summary>

        [Optional] public long? InstructionOffset { get; set; }

        /// <summary>
        /// Number of instructions to disassemble starting at the specified location and offset.An adapter must return exactly this number of instructions - any unavailable instructions should be replaced with an implementation-defined 'invalid instruction' value.
        /// </summary>
        public long InstructionCount { get; set; }

        /// <summary>
        /// If true, the adapter should attempt to resolve memory addresses and other values to symbolic names.
        /// </summary>
        [Optional] public bool? ResolveSymbols { get; set; }
    }

}
