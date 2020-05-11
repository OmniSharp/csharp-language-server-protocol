using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(JsonNumberEnumConverter))]
    public enum DiagnosticSeverity
    {
        /// <summary>
        /// Reports an error.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Reports a warning.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Reports an information.
        /// </summary>
        Information = 3,
        /// <summary>
        /// Reports a hint.
        /// </summary>
        Hint = 4,
    }
}
