using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceClientCapabilites
    {
        /// <summary>
        /// The client supports applying batch edits
        /// to the workspace.
        /// </summary>
        public Supports<bool> ApplyEdit { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeApplyEdit() => ApplyEdit.IsSupported;

        public Supports<WorkspaceEditCapability> WorkspaceEdit { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeWorkspaceEdit() => WorkspaceEdit.IsSupported;

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeConfiguration` notification.
        /// </summary>
        public Supports<DidChangeConfigurationCapability> DidChangeConfiguration { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDidChangeConfiguration() => DidChangeConfiguration.IsSupported;

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeWatchedFiles` notification.
        /// </summary>
        public Supports<DidChangeWatchedFilesCapability> DidChangeWatchedFiles { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDidChangeWatchedFiles() => DidChangeWatchedFiles.IsSupported;

        /// <summary>
        /// Capabilities specific to the `workspace/symbol` request.
        /// </summary>
        public Supports<WorkspaceSymbolCapability> Symbol { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSymbol() => Symbol.IsSupported;

        /// <summary>
        /// Capabilities specific to the `workspace/executeCommand` request.
        /// </summary>
        public Supports<ExecuteCommandCapability> ExecuteCommand { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeExecuteCommand() => ExecuteCommand.IsSupported;
    }
}
