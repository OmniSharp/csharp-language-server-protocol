namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SourcePresentationHint
    {
        Normal,
        Emphasize,
        Deemphasize,
    }
}