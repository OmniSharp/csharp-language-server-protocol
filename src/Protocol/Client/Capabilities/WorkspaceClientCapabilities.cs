namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceClientCapabilities
    {
        /// <summary>
        /// The client supports applying batch edits
        /// to the workspace.
        /// </summary>
        public Supports<bool> ApplyEdit { get; set; }

        public Supports<WorkspaceEditCapability> WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeConfiguration` notification.
        /// </summary>
        public Supports<DidChangeConfigurationCapability> DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeWatchedFiles` notification.
        /// </summary>
        public Supports<DidChangeWatchedFilesCapability> DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/symbol` request.
        /// </summary>
        public Supports<WorkspaceSymbolCapability> Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/executeCommand` request.
        /// </summary>
        public Supports<ExecuteCommandCapability> ExecuteCommand { get; set; }

        /// <summary>
        /// The client has support for workspace folders.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<bool> WorkspaceFolders { get; set; }

        /// <summary>
        /// The client supports `workspace/configuration` requests.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<bool> Configuration { get; set; }
    }
}
