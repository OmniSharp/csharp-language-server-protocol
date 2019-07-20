using System.Text;
using System.Threading;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class GotoTargetsResponse
    {
        /// <summary>
        /// The possible goto targets of the specified location.
        /// </summary>
        public Container<GotoTarget> Targets { get; set; }
    }

}
