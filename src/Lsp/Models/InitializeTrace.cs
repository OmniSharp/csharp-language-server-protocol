using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lsp.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitializeTrace
    {
        off,
        messages,
        verbose
    }
}