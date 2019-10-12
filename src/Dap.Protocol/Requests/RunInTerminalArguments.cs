using System.Collections.Generic;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class RunInTerminalArguments : IRequest<RunInTerminalResponse>
    {
        /// <summary>
        /// What kind of terminal to launch.
        /// </summary>
        [Optional] public RunInTerminalArgumentsKind Kind { get; set; }

        /// <summary>
        /// Optional title of the terminal.
        /// </summary>
        [Optional] public string Title { get; set; }

        /// <summary>
        /// Working directory of the command.
        /// </summary>
        public string Cwd { get; set; }

        /// <summary>
        /// List of arguments.The first argument is the command to run.
        /// </summary>
        public Container<string> Args { get; set; }

        /// <summary>
        /// Environment key-value pairs that are added to or removed from the default environment.
        /// </summary>
        [Optional] public IDictionary<string, string> Env { get; set; }

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
