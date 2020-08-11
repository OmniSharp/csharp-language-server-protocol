using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetFunctionBreakpointsResponse
    {
        /// <summary>
        /// Information about the breakpoints.The array elements correspond to the elements of the 'breakpoints' array.
        /// </summary>
        public Container<Breakpoint> Breakpoints { get; set; }
    }
}
