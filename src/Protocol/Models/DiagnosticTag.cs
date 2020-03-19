using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The diagnostic tags.
    ///
    /// @since 3.15.0
    /// </summary>
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum DiagnosticTag
    {
        /// <summary>
        /// Unused or unnecessary code.
        ///
        /// Clients are allowed to render diagnostics with this tag faded out instead of having
        /// an error squiggle.
        /// </summary>
        Unnecessary = 1,

        /// <summary>
        /// Deprecated or obsolete code.
        ///
        /// Clients are allowed to rendered diagnostics with this tag strike through.
        /// </summary>
        Deprecated = 2,
    }
}
