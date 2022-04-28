using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.RunInTerminal, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record RunInTerminalArguments : IRequest<RunInTerminalResponse>
        {
            /// <summary>
            /// What kind of terminal to launch.
            /// </summary>
            [Optional]
            public RunInTerminalArgumentsKind? Kind { get; init; }

            /// <summary>
            /// Optional title of the terminal.
            /// </summary>
            [Optional]
            public string? Title { get; init; }

            /// <summary>
            /// Working directory of the command.
            /// </summary>
            public string Cwd { get; init; } = null!;

            /// <summary>
            /// List of arguments.The first argument is the command to run.
            /// </summary>
            public Container<string> Args { get; init; } = null!;

            /// <summary>
            /// Environment key-value pairs that are added to or removed from the default environment.
            /// </summary>
            [Optional]
            public IDictionary<string, string>? Env { get; init; }
        }

        public record RunInTerminalResponse
        {
            /// <summary>
            /// The process ID.
            /// </summary>
            [Optional]
            public long? ProcessId { get; init; }

            /// <summary>
            /// The process ID of the terminal shell.
            /// </summary>
            [Optional]
            public long? ShellProcessId { get; init; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum RunInTerminalArgumentsKind
        {
            Integrated,
            External
        }
    }
}
