using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitializeTrace
    {
        [EnumMember(Value="off")]
        Off,
        [EnumMember(Value="messages")]
        Messages,
        [EnumMember(Value="verbose")]
        Verbose
    }
}
