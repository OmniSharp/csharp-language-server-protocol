using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RunInTerminalArgumentsKind
    {
        Integrated,
        External
    }

}
