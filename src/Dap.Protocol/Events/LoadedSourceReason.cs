using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LoadedSourceReason
    {
        New, Changed, Removed
    }
}
