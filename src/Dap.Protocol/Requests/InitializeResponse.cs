using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class InitializeResponse
    {
        /// <summary>
        /// The capabilities of this debug adapter.
        /// </summary>
        [Optional] public Capabilities Body { get; set; }
    }

}
