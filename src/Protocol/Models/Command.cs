using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class Command
    {
        /// <summary>
        /// Title of the command, like `save`.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        [JsonProperty("command")]
        public string Name { get; set; }

        /// <summary>
        /// Arguments that the command handler should be
        /// invoked with.
        /// </summary>
        [Optional]
        public JArray Arguments { get; set; }
    }
}
