using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The kind of a completion entry.
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum CompletionItemKind
    {
        Text = 1,
        Method = 2,
        Function = 3,
        Constructor = 4,
        Field = 5,
        Variable = 6,
        Class = 7,
        Interface = 8,
        Module = 9,
        Property = 10,
        Unit = 11,
        Value = 12,
        Enum = 13,
        Keyword = 14,
        Snippet = 15,
        Color = 16,
        File = 17,
        Reference = 18,
    }
}