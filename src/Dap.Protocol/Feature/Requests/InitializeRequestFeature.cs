using MediatR;
using Newtonsoft.Json;
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
        [Method(RequestNames.Initialize, Direction.ClientToServer)]
        [
            GenerateHandler(Name = "DebugAdapterInitialize"),
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record InitializeRequestArguments : IRequest<InitializeResponse>, IInitializeRequestArguments
        {
            /// <summary>
            /// The ID of the(frontend) client using this adapter.
            /// </summary>

            [Optional, JsonProperty("clientID")]
            public string? ClientId { get; set; }

            /// <summary>
            /// The human readable name of the(frontend) client using this adapter.
            /// </summary>

            [Optional]
            public string? ClientName { get; set; }

            /// <summary>
            /// The ID of the debug adapter.
            /// </summary>
            [JsonProperty("adapterID")]
            public string AdapterId { get; set; } = null!;

            /// <summary>
            /// The ISO-639 locale of the(frontend) client using this adapter, e.g.en-US or de-CH.
            /// </summary>

            [Optional]
            public string? Locale { get; set; }

            /// <summary>
            /// If true all line numbers are 1-based(default).
            /// </summary>
            [Optional]
            public bool LinesStartAt1 { get; set; }

            /// <summary>
            /// If true all column numbers are 1-based(default).
            /// </summary>
            [Optional]
            public bool ColumnsStartAt1 { get; set; }

            /// <summary>
            /// Determines in what format paths are specified.The default is 'path', which is the native format.
            /// Values: 'path', 'uri', etc.
            /// </summary>
            [Optional]
            public PathFormat? PathFormat { get; set; }

            /// <summary>
            /// Client supports the optional type attribute for variables.
            /// </summary>
            [Optional]
            public bool SupportsVariableType { get; set; }

            /// <summary>
            /// Client supports the paging of variables.
            /// </summary>
            [Optional]
            public bool SupportsVariablePaging { get; set; }

            /// <summary>
            /// Client supports the runInTerminal request.
            /// </summary>
            [Optional]
            public bool SupportsRunInTerminalRequest { get; set; }

            /// <summary>
            /// Client supports memory references.
            /// </summary>
            [Optional]
            public bool SupportsMemoryReferences { get; set; }

            /// <summary>
            /// Client supports progress reporting.
            /// </summary>
            [Optional]
            public bool SupportsProgressReporting { get; set; }

            /// <summary>
            /// Client supports the invalidated event.
            /// </summary>
            [Optional]
            public bool SupportsInvalidatedEvent { get; set; }
        }

        public record InitializeResponse : Capabilities
        {
        }
    }
}
