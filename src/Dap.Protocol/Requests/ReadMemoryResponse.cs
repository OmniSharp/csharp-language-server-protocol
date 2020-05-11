using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ReadMemoryResponse
    {
        /// <summary>
        /// The address of the first byte of data returned.Treated as a hex value if prefixed with '0x', or as a decimal value otherwise.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The number of unreadable bytes encountered after the last successfully read byte. This can be used to determine the number of bytes that must be skipped before a subsequent 'readMemory' request will succeed.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? UnreadableBytes { get; set; }

        /// <summary>
        /// The bytes read from memory, encoded using base64.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Data { get; set; }
    }

}
