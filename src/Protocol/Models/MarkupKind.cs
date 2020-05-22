using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Describes the content type that a client supports in various
    /// result literals like `Hover`, `ParameterInfo` or `CompletionItem`.
    ///
    /// Please note that `MarkupKinds` must not start with a `$`. This kinds
    /// are reserved for internal usage.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarkupKind
    {
        /// <summary>
        /// Plain text is supported as a content format
        /// </summary>
        [EnumMember(Value="plaintext")]
        PlainText, // Only capitalize the first letter because the above converter will only lower case the first letter today
        /// <summary>
        /// Markdown is supported as a content format
        /// </summary>
        [EnumMember(Value="markdown")]
        Markdown,
    }
}
