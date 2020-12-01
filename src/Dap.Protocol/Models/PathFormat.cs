using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [StringEnum]
    public readonly partial struct PathFormat
    {
        public static PathFormat Path { get; } = new PathFormat("path");
        public static PathFormat Uri { get; } = new PathFormat("uri");
    }
}
