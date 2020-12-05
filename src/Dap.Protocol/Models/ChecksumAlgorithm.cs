using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// Names of checksum algorithms that may be supported by a debug adapter.
    /// </summary>
    [StringEnum]
    public readonly partial struct  ChecksumAlgorithm
    {
        public static ChecksumAlgorithm Md5 { get; } = new ChecksumAlgorithm("MD5");
        public static ChecksumAlgorithm Sha1 { get; } = new ChecksumAlgorithm("SHA1");
        public static ChecksumAlgorithm Sha256 { get; } = new ChecksumAlgorithm("SHA256");
        public static ChecksumAlgorithm Timestamp { get; } = new ChecksumAlgorithm("timestamp");
    }
}
