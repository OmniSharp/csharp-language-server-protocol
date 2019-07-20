using System.Collections.Generic;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class RunInTerminalArguments : IRequest<RunInTerminalResponse>
    {
        /// <summary>
        /// What kind of terminal to launch.
        /// </summary>
        [Optional] public RunInTerminalArgumentsKind kind { get; set; }

        /// <summary>
        /// Optional title of the terminal.
        /// </summary>
        [Optional] public string title { get; set; }

        /// <summary>
        /// Working directory of the command.
        /// </summary>
        public string cwd { get; set; }

        /// <summary>
        /// List of arguments.The first argument is the command to run.
        /// </summary>
        public Container<string> args { get; set; }

        /// <summary>
        /// Environment key-value pairs that are added to or removed from the default environment.
        /// </summary>
        [Optional] public IDictionary<string, string> env { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
