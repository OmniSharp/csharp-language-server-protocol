using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Rename file Options
    /// </summary>
    public class RenameFileOptions
    {
        /// <summary>
        /// Overwrite target if existing. Overwrite wins over `ignoreIfExists`
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool Overwrite { get; set; }
        /// <summary>
        /// Ignores if target exists.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool IgnoreIfExists { get; set; }
    }
}
