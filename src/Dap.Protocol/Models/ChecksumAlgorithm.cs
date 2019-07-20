using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>
    /// Names of checksum algorithms that may be supported by a debug adapter.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChecksumAlgorithm
    {
        Md5, Sha1, Sha256, Timestamp
    }
}