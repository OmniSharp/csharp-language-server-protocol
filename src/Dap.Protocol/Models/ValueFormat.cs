using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Provides formatting information for a value.
    /// </summary>
    public record ValueFormat
    {
        /// <summary>
        /// Display the value in hex.
        /// </summary>
        [Optional]
        public bool Hex { get; init; }
    }
}
