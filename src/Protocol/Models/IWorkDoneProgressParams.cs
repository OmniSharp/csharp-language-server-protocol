using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IWorkDoneProgressParams
    {
        /// <summary>
        /// An optional token that a server can use to report work done progress.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        ProgressToken WorkDoneToken { get; set; }
    }
}
