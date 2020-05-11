using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModuleEventReason
    {
        New, Changed, Removed
    }

}
