using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [StringEnum]
    public readonly partial struct ColumnDescriptorType
    {
        public static ColumnDescriptorType String { get; } = new ColumnDescriptorType("string");
        public static ColumnDescriptorType Long { get; } = new ColumnDescriptorType("long");
        public static ColumnDescriptorType Bool { get; } = new ColumnDescriptorType("boolean");
        public static ColumnDescriptorType UnixTimestampUtc { get; } = new ColumnDescriptorType("unixTimestampUTC");
    }
}
