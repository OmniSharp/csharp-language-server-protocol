using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
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

        private string DebuggerDisplay =>
            $"{Title}{(string.IsNullOrWhiteSpace(Name) ? "" : $" {Name}")}{(Arguments == null ? "" : string.Join(", ", Arguments.Select(z => z.ToString().Trim('"'))))}";

        public override string ToString() => DebuggerDisplay;

        public Command WithCommand(string command)
        {
            Name = command;
            return this;
        }

        public Command WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public Command WithArguments(params object[] args)
        {
            Arguments = JArray.FromObject(args);
            return this;
        }

        public static Command Create(string name, params object[] args) => new Command() {
            Name = name,
            Arguments = JArray.FromObject(args)
        };
    }
}
