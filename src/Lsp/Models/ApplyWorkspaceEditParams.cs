using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ApplyWorkspaceEditParams
    {
        /// <summary>
        /// The edits to apply.
        /// </summary>
        public WorkspaceEdit Edit { get; set; }
    }
}