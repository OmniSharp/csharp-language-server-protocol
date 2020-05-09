namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
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