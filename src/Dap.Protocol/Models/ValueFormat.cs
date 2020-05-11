using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Provides formatting information for a value.
    /// </summary>
    public class ValueFormat
    {
        /// <summary>
        /// Display the value in hex.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? Hex { get; set; }
    }
}
