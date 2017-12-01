using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(LowercaseStringEnumConverter))]
    public enum InitializeTrace
    {
        Off,
        Messages,
        Verbose
    }
}
