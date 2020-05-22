using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Initialize, Direction.ClientToServer)]
    public class InitializeRequestArguments : IRequest<InitializeResponse>
    {
        /// <summary>
        /// The ID of the(frontend) client using this adapter.
        /// </summary>

        [Optional] public string ClientId { get; set; }

        /// <summary>
        /// The human readable name of the(frontend) client using this adapter.
        /// </summary>

        [Optional] public string ClientName { get; set; }

        /// <summary>
        /// The ID of the debug adapter.
        /// </summary>
        public string AdapterId { get; set; }

        /// <summary>
        /// The ISO-639 locale of the(frontend) client using this adapter, e.g.en-US or de-CH.
        /// </summary>

        [Optional] public string Locale { get; set; }

        /// <summary>
        /// If true all line numbers are 1-based(default).
        /// </summary>
        [Optional] public bool? LinesStartAt1 { get; set; }

        /// <summary>
        /// If true all column numbers are 1-based(default).
        /// </summary>
        [Optional] public bool? ColumnsStartAt1 { get; set; }

        /// <summary>
        /// Determines in what format paths are specified.The default is 'path', which is the native format.

        /// Values: 'path', 'uri', etc.
        /// </summary>
        [Optional] public string PathFormat { get; set; }

        /// <summary>
        /// Client supports the optional type attribute for variables.
        /// </summary>
        [Optional] public bool? SupportsVariableType { get; set; }

        /// <summary>
        /// Client supports the paging of variables.
        /// </summary>
        [Optional] public bool? SupportsVariablePaging { get; set; }

        /// <summary>
        /// Client supports the runInTerminal request.
        /// </summary>
        [Optional] public bool? SupportsRunInTerminalRequest { get; set; }

        /// <summary>
        /// Client supports memory references.
        /// </summary>
        [Optional] public bool? SupportsMemoryReferences { get; set; }
    }

}
