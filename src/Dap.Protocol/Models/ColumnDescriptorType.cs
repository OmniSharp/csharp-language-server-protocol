using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColumnDescriptorType
    {
        String,

        Long,
        Bool,
        UnixTimestampUtc,
    }
}