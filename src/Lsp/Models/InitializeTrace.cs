using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitializeTrace
    {
        off,
        messages,
        verbose
    }
}