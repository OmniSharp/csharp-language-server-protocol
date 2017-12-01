using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Execute command registration options.
    /// </summary>
    public class ExecuteCommandRegistrationOptions : IExecuteCommandOptions
    {
        /// <summary>
        /// The commands to be executed on the server
        /// </summary>
        public Container<string> Commands { get; set; }
    }
}
