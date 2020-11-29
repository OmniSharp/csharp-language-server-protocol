using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record Command
    {
        /// <summary>
        /// Title of the command, like `save`.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        [JsonProperty("command")]
        public string Name { get; init; }

        /// <summary>
        /// Arguments that the command handler should be
        /// invoked with.
        /// </summary>
        [Optional]
        public JArray? Arguments { get; init; }

        private string DebuggerDisplay =>
            $"{Title}{( string.IsNullOrWhiteSpace(Name) ? "" : $" {Name}" )}{( Arguments == null ? "" : string.Join(", ", Arguments.Select(z => z.ToString().Trim('"'))) )}";

        public override string ToString() => DebuggerDisplay;

        public Command WithArguments(params object[] args) => this with { Arguments = JArray.FromObject(args) };
        public static Command Create(string name, params object[] args) => new() { Name = name, Arguments = JArray.FromObject(args) };
    }
}
