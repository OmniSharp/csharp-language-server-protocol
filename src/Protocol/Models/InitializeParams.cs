using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Initialize, Direction.ClientToServer)]
    public class InitializeParams : IWorkDoneProgressParams, IRequest<InitializeResult>
    {
        /// <summary>
        /// The process Id of the parent process that started
        /// the server. Is null if the process has not been started by another process.
        /// If the parent process is not alive then the server should exit (see exit notification) its process.
        /// </summary>
        public long? ProcessId { get; set; }

        /// <summary>
        /// Information about the client
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// The rootPath of the workspace. Is null
        /// if no folder is open.
        ///
        /// @deprecated in favour of rootUri.
        /// </summary>
        [Optional]
        public string RootPath
        {
            get { return RootUri?.GetFileSystemPath(); }
            set { RootUri = value == null ? null : DocumentUri.FromFileSystemPath(value); }
        }

        /// <summary>
        /// The rootUri of the workspace. Is null if no
        /// folder is open. If both `rootPath` and `rootUri` are set
        /// `rootUri` wins.
        /// </summary>
        public DocumentUri RootUri { get; set; }

        /// <summary>
        /// User provided initialization options.
        /// </summary>
        public object InitializationOptions { get; set; }

        /// <summary>
        /// The capabilities provided by the client (editor or tool)
        /// </summary>
        public ClientCapabilities Capabilities { get; set; }

        /// <summary>
        /// The initial trace setting. If omitted trace is disabled ('off').
        /// </summary>
        [Optional]
        public InitializeTrace Trace { get; set; } = InitializeTrace.Off;

        /// <summary>
        /// The workspace folders configured in the client when the server starts.
        /// This property is only available if the client supports workspace folders.
        /// It can be `null` if the client supports workspace folders but none are
        /// configured.
        ///
        /// Since 3.6.0
        /// <summary/>
        public Container<WorkspaceFolder> WorkspaceFolders { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
