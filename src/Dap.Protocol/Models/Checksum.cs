namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// The checksum of an item calculated by the specified algorithm.
    /// </summary>

    public class Checksum
    {
        /// <summary>
        /// The algorithm used to calculate this checksum.
        /// </summary>
        public ChecksumAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Value of the checksum.
        /// </summary>
        [JsonProperty("checksum")]
        public string Value { get; set; }
    }
}