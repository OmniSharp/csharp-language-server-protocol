using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Delete file Options
    /// </summary>
    public class DeleteFileOptions
    {
        /// <summary>
        /// Delete the content recursively if a folder is denoted.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool Recursive { get; set; }
        /// <summary>
        /// Ignore the operation if the file doesn't exist.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool IgnoreIfNotExists { get; set; }
    }
}
