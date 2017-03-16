using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Server
{
    /// <summary>
    ///  Execute command options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ExecuteCommandOptions
    {
        /// <summary>
        ///  The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }
    }
}