using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Server
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

        public static ExecuteCommandOptions Of(IEnumerable<IExecuteCommandOptions> options)
        {
            return new ExecuteCommandOptions() { Commands = options.SelectMany(x => x.Commands).ToArray() };
        }
    }
}
