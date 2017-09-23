using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    /// <summary>
    /// Contains additional diagnostic information about the context in which
    /// a code action is run.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CodeActionContext
    {
        /// <summary>
        /// An array of diagnostics.
        /// </summary>
        public Container<Diagnostic> Diagnostics { get; set; }
    }
}