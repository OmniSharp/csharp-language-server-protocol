using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModuleEventReason
    {
        New, Changed, Removed
    }
}
