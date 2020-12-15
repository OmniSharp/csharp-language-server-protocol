using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class WorkspaceNames
    {
        public const string ApplyEdit = "workspace/applyEdit";
        public const string Configuration = "workspace/configuration";
        public const string DidChangeConfiguration = "workspace/didChangeConfiguration";
        public const string DidChangeWatchedFiles = "workspace/didChangeWatchedFiles";
        public const string DidChangeWorkspaceFolders = "workspace/didChangeWorkspaceFolders";
        public const string ExecuteCommand = "workspace/executeCommand";
        public const string WorkspaceSymbol = "workspace/symbol";
        public const string WorkspaceFolders = "workspace/workspaceFolders";
        [Obsolete(Constants.Proposal)]
        public const string SemanticTokensRefresh = "workspace/semanticTokens/refresh";
        [Obsolete(Constants.Proposal)]
        public const string CodeLensRefresh = "workspace/codeLens/refresh";
        [Obsolete(Constants.Proposal)]
        public const string WillCreateFiles = "workspace/willCreateFiles";
        [Obsolete(Constants.Proposal)]
        public const string DidCreateFiles = "workspace/didCreateFiles";
        [Obsolete(Constants.Proposal)]
        public const string WillRenameFiles = "workspace/willRenameFiles";
        [Obsolete(Constants.Proposal)]
        public const string DidRenameFiles = "workspace/didRenameFiles";
        [Obsolete(Constants.Proposal)]
        public const string WillDeleteFiles = "workspace/willDeleteFiles";
        [Obsolete(Constants.Proposal)]
        public const string DidDeleteFiles = "workspace/didDeleteFiles";

    }
}
