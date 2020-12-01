using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [StringEnum]
    public readonly partial struct InvalidatedAreas
    {
        public static InvalidatedAreas All = new InvalidatedAreas("all");
        public static InvalidatedAreas Stacks = new InvalidatedAreas("stacks");
        public static InvalidatedAreas Threads = new InvalidatedAreas("threads");
        public static InvalidatedAreas Variables = new InvalidatedAreas("variables");
    }
}
