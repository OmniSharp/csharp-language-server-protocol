using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetInstructionBreakpointsResponse
    {
        /// <summary>
        /// Information about the data breakpoints.The array elements correspond to the elements of the input argument 'breakpoints' array.
        /// </summary>
        public Container<Breakpoint> Breakpoints { get; set; } = null!;
    }
}
