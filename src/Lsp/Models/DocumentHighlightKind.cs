using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Converters;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// A document highlight kind.
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DocumentHighlightKind
    {
        /// <summary>
        /// A textual occurrence.
        /// </summary>
        Text = 1,

        /// <summary>
        /// Read-access of a symbol, like reading a variable.
        /// </summary>
        Read = 2,

        /// <summary>
        /// Write-access of a symbol, like writing to a variable.
        /// </summary>
        Write = 3,
    }
}