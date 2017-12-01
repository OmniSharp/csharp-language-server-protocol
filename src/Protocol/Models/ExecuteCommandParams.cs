using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [Optional]
        public JArray Arguments { get; set; }
    }
}
