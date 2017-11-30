using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    /// The parameters of a Workspace Symbol Request.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WorkspaceSymbolParams
    {
        /// <summary>
        /// A non-empty query string
        /// </summary>
        public string Query { get; set; }
    }
}