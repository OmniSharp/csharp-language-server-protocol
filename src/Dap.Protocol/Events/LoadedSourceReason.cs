using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LoadedSourceReason
    {
        New, Changed, Removed
    }
}
