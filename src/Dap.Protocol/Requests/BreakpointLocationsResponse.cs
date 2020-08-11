using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class BreakpointLocationsResponse
    {
        /// <summary>
        /// Sorted set of possible breakpoint locations.
        /// </summary>
        public Container<BreakpointLocation> Breakpoints { get; set; }
    }
}
