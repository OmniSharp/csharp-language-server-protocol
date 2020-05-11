using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StackFramePresentationHint
    {
        Normal,
        Label,
        Subtle,
    }
}
