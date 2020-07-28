namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public interface IInitializeRequestArguments
    {
        /// <summary>
        /// The ID of the(frontend) client using this adapter.
        /// </summary>
        string ClientId { get; set; }

        /// <summary>
        /// The human readable name of the(frontend) client using this adapter.
        /// </summary>
        string ClientName { get; set; }

        /// <summary>
        /// The ID of the debug adapter.
        /// </summary>
        string AdapterId { get; set; }

        /// <summary>
        /// The ISO-639 locale of the(frontend) client using this adapter, e.g.en-US or de-CH.
        /// </summary>
        string Locale { get; set; }

        /// <summary>
        /// If true all line numbers are 1-based(default).
        /// </summary>
        bool? LinesStartAt1 { get; set; }

        /// <summary>
        /// If true all column numbers are 1-based(default).
        /// </summary>
        bool? ColumnsStartAt1 { get; set; }

        /// <summary>
        /// Determines in what format paths are specified.The default is 'path', which is the native format.
        /// Values: 'path', 'uri', etc.
        /// </summary>
        string PathFormat { get; set; }

        /// <summary>
        /// Client supports the optional type attribute for variables.
        /// </summary>
        bool? SupportsVariableType { get; set; }

        /// <summary>
        /// Client supports the paging of variables.
        /// </summary>
        bool? SupportsVariablePaging { get; set; }

        /// <summary>
        /// Client supports the runInTerminal request.
        /// </summary>
        bool? SupportsRunInTerminalRequest { get; set; }

        /// <summary>
        /// Client supports memory references.
        /// </summary>
        bool? SupportsMemoryReferences { get; set; }

        /// <summary>
        /// Client supports progress reporting.
        /// </summary>
        bool? SupportsProgressReporting { get; set; }
    }
}
