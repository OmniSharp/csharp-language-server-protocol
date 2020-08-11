using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class DisassembleResponse
    {
        /// <summary>
        /// The list of disassembled instructions.
        /// </summary>
        public Container<DisassembledInstruction> Instructions { get; set; }
    }
}
