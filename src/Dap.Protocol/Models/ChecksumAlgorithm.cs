using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Names of checksum algorithms that may be supported by a debug adapter.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChecksumAlgorithm
    {
        Md5, Sha1, Sha256, Timestamp
    }

    [StringEnum]
    public readonly partial struct PathFormat
    {
        public static PathFormat Path { get; } = new PathFormat("path");
        public static PathFormat Uri { get; } = new PathFormat("uri");
    }
}
