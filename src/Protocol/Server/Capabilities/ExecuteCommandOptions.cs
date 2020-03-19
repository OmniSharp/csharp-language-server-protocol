using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Execute command options.
    /// </summary>
    public class ExecuteCommandOptions : WorkDoneProgressOptions, IExecuteCommandOptions
    {
        /// <summary>
        ///  The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }

        public static ExecuteCommandOptions Of(IEnumerable<IExecuteCommandOptions> options)
        {
            return new ExecuteCommandOptions() {
                Commands = options.SelectMany(x => x.Commands).ToArray(),
                WorkDoneProgress = options.Any(x => x.WorkDoneProgress)
            };
        }
    }
}
