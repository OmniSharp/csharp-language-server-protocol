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

        public static Command Create(string name, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray()
        };

        public static Command Create<T>(string name, T arg, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg)
        };

        public static Command Create<T, T2>(string name, T arg, T2 arg2, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg, arg2)
        };

        public static Command Create<T, T2, T3>(string name, T arg, T2 arg2, T3 arg3, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg, arg2, arg3)
        };

        public static Command Create<T, T2, T3, T4>(string name, T arg, T2 arg2, T3 arg3, T4 arg4, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg, arg2, arg3, arg4)
        };

        public static Command Create<T, T2, T3, T4, T5>(string name, T arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg, arg2, arg3, arg4, arg5)
        };

        public static Command Create<T, T2, T3, T4, T5, T6>(string name, T arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, string title = null) => new Command() {
            Name = name,
            Title = title,
            Arguments = new JArray(arg, arg2, arg3, arg4, arg5, arg6)
        };
    }
}
