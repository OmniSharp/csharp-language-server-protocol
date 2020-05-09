using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Completion item tags are extra annotations that tweak the rendering of a completion
    /// item.
    ///
    /// @since 3.15.0
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum CompletionItemTag
    {
        /// <summary>
        /// Render a completion as obsolete, usually using a strike-out.
        /// </summary>
        Deprecated = 1
    }
}
