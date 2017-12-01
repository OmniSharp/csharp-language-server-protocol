using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Describes the content type that a client supports in various
    /// result literals like `Hover`, `ParameterInfo` or `CompletionItem`.
    ///
    /// Please note that `MarkupKinds` must not start with a `$`. This kinds
    /// are reserved for internal usage.
    /// </summary>
    [JsonConverter(typeof(LowercaseStringEnumConverter))]
    public enum MarkupKind
    {
        /// <summary>
        /// Plain text is supported as a content format
        /// </summary>
        Plaintext, // Only capitalize the first letter because the above converter will only lower case the first letter today
        /// <summary>
        /// Markdown is supported as a content format
        /// </summary>
        Markdown,
    }
}
