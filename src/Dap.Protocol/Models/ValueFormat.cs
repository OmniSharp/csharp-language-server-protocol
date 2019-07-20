using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// Provides formatting information for a value.
    /// </summary>
    public class ValueFormat
    {
        /// <summary>
        /// Display the value in hex.
        /// </summary>
        [Optional] public bool? Hex { get; set; }
    }
}