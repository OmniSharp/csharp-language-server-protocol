using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server
{
    /// <summary>
    ///  Execute command options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ExecuteCommandOptions : IExecuteCommandOptions
    {
        /// <summary>
        ///  The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }

        public static ExecuteCommandOptions Of(IExecuteCommandOptions options)
        {
            return new ExecuteCommandOptions() { Commands = options.Commands };
        }
    }
}