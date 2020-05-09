using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Defines whether the insert text in a completion item should be interpreted as
    /// plain text or a snippet.
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum InsertTextFormat
    {
        /// <summary>
        /// The primary text to be inserted is treated as a plain string.
        /// </summary>
        PlainText = 1,

        /// <summary>
        /// The primary text to be inserted is treated as a snippet.
        ///
        /// A snippet can define tab stops and placeholders with `$1`, `$2`
        /// and `${3:foo}`. `$0` defines the final tab stop, it defaults to
        /// the end of the snippet. Placeholders with equal identifiers are linked,
        /// that is typing in one will update others too.
        ///
        /// See also: https://github.com/Microsoft/vscode/blob/master/src/vs/editor/contrib/snippet/common/snippet.md
        /// </summary>
        Snippet = 2,
    }
}
