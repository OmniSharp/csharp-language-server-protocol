namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class DisassembleResponse
    {
        /// <summary>
        /// The list of disassembled instructions.
        /// </summary>
        public Container<DisassembledInstruction> Instructions { get; set; }
    }

}
