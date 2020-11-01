using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Initialize, Direction.ClientToServer)]
    public class InitializeParams : IInitializeParams<ClientCapabilities>
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
        [DisallowNull]
        public ClientInfo? ClientInfo { get; set; }

        /// <summary>
        /// The rootPath of the workspace. Is null
        /// if no folder is open.
        ///
        /// @deprecated in favour of rootUri.
        /// </summary>
        [Optional]
        [DisallowNull]
        public string? RootPath
        {
            get => RootUri?.GetFileSystemPath();
            set {
                if (!string.IsNullOrEmpty(value))
                {
                    RootUri = DocumentUri.FromFileSystemPath(value!);
                }
            }
        }

        /// <summary>
        /// The rootUri of the workspace. Is null if no
        /// folder is open. If both `rootPath` and `rootUri` are set
        /// `rootUri` wins.
        /// </summary>
        [DisallowNull]
        public DocumentUri? RootUri { get; set; }

        /// <summary>
        /// User provided initialization options.
        /// </summary>
        [DisallowNull]
        public object? InitializationOptions { get; set; }

        /// <summary>
        /// The capabilities provided by the client (editor or tool)
        /// </summary>
        [MaybeNull] public ClientCapabilities Capabilities { get; set; } = null!;

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
        /// </summary>
        [MaybeNull]
        public Container<WorkspaceFolder>? WorkspaceFolders { get; set; }

        /// <inheritdoc />
        [Optional]
        [MaybeNull]
        public ProgressToken? WorkDoneToken { get; set; }

        public InitializeParams()
        {

        }

        internal InitializeParams(IInitializeParams<JObject> @params, ClientCapabilities clientCapabilities)
        {
            ProcessId = @params.ProcessId;
            Trace = @params.Trace;
            Capabilities = clientCapabilities;
            ClientInfo = @params.ClientInfo!;
            InitializationOptions = @params.InitializationOptions!;
            RootUri = @params.RootUri!;
            WorkspaceFolders = @params.WorkspaceFolders!;
            WorkDoneToken = @params.WorkDoneToken!;
        }
    }
}
