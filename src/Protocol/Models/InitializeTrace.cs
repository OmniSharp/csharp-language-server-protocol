using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
