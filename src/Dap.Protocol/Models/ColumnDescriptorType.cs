using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ColumnDescriptorType
    {
        String,

        Long,
        Bool,
        UnixTimestampUtc,
    }
}
