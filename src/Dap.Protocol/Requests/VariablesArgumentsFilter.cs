using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VariablesArgumentsFilter
    {
        Indexed, Named
    }

}
