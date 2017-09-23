using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    /// <summary>
    /// Execute command registration options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ExecuteCommandRegistrationOptions : IExecuteCommandOptions
    {
        /// <summary>
        /// The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }
    }
}