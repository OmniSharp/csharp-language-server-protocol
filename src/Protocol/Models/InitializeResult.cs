using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class InitializeResult
    {
        /// <summary>
        /// The capabilities the language server provides.
        /// </summary>
        public ServerCapabilities Capabilities { get; set; }
    }
}