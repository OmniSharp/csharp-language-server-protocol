using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IRenameOptions : IWorkDoneProgressOptions
    {
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        bool PrepareProvider { get; set; }
    }
}
