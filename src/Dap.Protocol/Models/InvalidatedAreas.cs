using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [StringEnum]
    public readonly partial struct InvalidatedAreas
    {
        public static InvalidatedAreas All { get; } = new InvalidatedAreas("all");
        public static InvalidatedAreas Stacks { get; } = new InvalidatedAreas("stacks");
        public static InvalidatedAreas Threads { get; } = new InvalidatedAreas("threads");
        public static InvalidatedAreas Variables { get; } = new InvalidatedAreas("variables");
    }
}
