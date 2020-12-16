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
        public const string SemanticTokensRefresh = "workspace/semanticTokens/refresh";
        public const string CodeLensRefresh = "workspace/codeLens/refresh";
        public const string WillCreateFiles = "workspace/willCreateFiles";
        public const string DidCreateFiles = "workspace/didCreateFiles";
        public const string WillRenameFiles = "workspace/willRenameFiles";
        public const string DidRenameFiles = "workspace/didRenameFiles";
        public const string WillDeleteFiles = "workspace/willDeleteFiles";
        public const string DidDeleteFiles = "workspace/didDeleteFiles";

    }
}
