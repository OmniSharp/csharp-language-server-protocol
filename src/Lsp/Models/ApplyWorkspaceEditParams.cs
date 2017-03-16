using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
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