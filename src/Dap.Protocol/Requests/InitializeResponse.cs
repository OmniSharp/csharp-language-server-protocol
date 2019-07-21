using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class InitializeResponse
    {
        /// <summary>
        /// The capabilities of this debug adapter.
        /// </summary>
        [Optional] public Capabilities Body { get; set; }
    }

}
