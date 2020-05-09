namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StackFramePresentationHint
    {
        Normal,
        Label,
        Subtle,
    }
}