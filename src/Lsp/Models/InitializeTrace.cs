using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitializeTrace
    {
        off,
        messages,
        verbose
    }
}