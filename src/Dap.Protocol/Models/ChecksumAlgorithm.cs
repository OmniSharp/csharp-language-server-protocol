using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Names of checksum algorithms that may be supported by a debug adapter.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChecksumAlgorithm
    {
        Md5, Sha1, Sha256, Timestamp
    }
}
