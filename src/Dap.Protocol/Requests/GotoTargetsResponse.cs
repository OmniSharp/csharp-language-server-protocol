using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class GotoTargetsResponse
    {
        /// <summary>
        /// The possible goto targets of the specified location.
        /// </summary>
        public Container<GotoTarget> Targets { get; set; } = null!;
    }
}
