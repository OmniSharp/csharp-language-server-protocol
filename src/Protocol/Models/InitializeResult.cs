using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
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