using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class InitializeRequestArguments : IRequest<InitializeResponse>
    {
        /// <summary>
        /// The ID of the(frontend) client using this adapter.
        /// </summary>

        [Optional] public string clientID { get; set; }

        /// <summary>
        /// The human readable name of the(frontend) client using this adapter.
        /// </summary>

        [Optional] public string clientName { get; set; }

        /// <summary>
        /// The ID of the debug adapter.
        /// </summary>
        public string adapterID { get; set; }

        /// <summary>
        /// The ISO-639 locale of the(frontend) client using this adapter, e.g.en-US or de-CH.
        /// </summary>

        [Optional] public string locale { get; set; }

        /// <summary>
        /// If true all line numbers are 1-based(default).
        /// </summary>
        [Optional] public bool? linesStartAt1 { get; set; }

        /// <summary>
        /// If true all column numbers are 1-based(default).
        /// </summary>
        [Optional] public bool? columnsStartAt1 { get; set; }

        /// <summary>
        /// Determines in what format paths are specified.The default is 'path', which is the native format.

        /// Values: 'path', 'uri', etc.
        /// </summary>
        [Optional] public string pathFormat { get; set; }

        /// <summary>
        /// Client supports the optional type attribute for variables.
        /// </summary>
        [Optional] public bool? supportsVariableType { get; set; }

        /// <summary>
        /// Client supports the paging of variables.
        /// </summary>
        [Optional] public bool? supportsVariablePaging { get; set; }

        /// <summary>
        /// Client supports the runInTerminal request.
        /// </summary>
        [Optional] public bool? supportsRunInTerminalRequest { get; set; }

        /// <summary>
        /// Client supports memory references.
        /// </summary>
        [Optional] public bool? supportsMemoryReferences { get; set; }
    }

}
