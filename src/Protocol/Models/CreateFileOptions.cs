using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Options to create a file.
    /// </summary>
    public class CreateFileOptions
    {
        /// <summary>
        /// Overwrite existing file. Overwrite wins over `ignoreIfExists`
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool Overwrite { get; set; }
        /// <summary>
        /// Ignore if exists.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool IgnoreIfExists { get; set; }
    }
}
