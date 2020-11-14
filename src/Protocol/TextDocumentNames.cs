using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class TextDocumentNames
    {
        public const string CodeAction = "textDocument/codeAction";
        public const string CodeActionResolve = "codeAction/resolve";
        public const string CodeLens = "textDocument/codeLens";
        public const string CodeLensResolve = "codeLens/resolve";
        public const string ColorPresentation = "textDocument/colorPresentation";
        public const string Completion = "textDocument/completion";
        public const string CompletionResolve = "completionItem/resolve";
        public const string Definition = "textDocument/definition";
        public const string Declaration = "textDocument/declaration";
        public const string DidChange = "textDocument/didChange";
        public const string DidClose = "textDocument/didClose";
        public const string DidOpen = "textDocument/didOpen";
        public const string DidSave = "textDocument/didSave";
        public const string DocumentColor = "textDocument/documentColor";
        public const string DocumentHighlight = "textDocument/documentHighlight";
        public const string DocumentFormatting = "textDocument/formatting";
        public const string DocumentLink = "textDocument/documentLink";
        public const string DocumentLinkResolve = "documentLink/resolve";
        public const string OnTypeFormatting = "textDocument/onTypeFormatting";
        public const string RangeFormatting = "textDocument/rangeFormatting";
        public const string DocumentSymbol = "textDocument/documentSymbol";
        public const string Hover = "textDocument/hover";
        public const string Implementation = "textDocument/implementation";
        public const string References = "textDocument/references";
        public const string Rename = "textDocument/rename";
        public const string PrepareRename = "textDocument/prepareRename";
        public const string SelectionRange = "textDocument/selectionRange";
        public const string SignatureHelp = "textDocument/signatureHelp";
        public const string TypeDefinition = "textDocument/typeDefinition";
        public const string WillSave = "textDocument/willSave";
        public const string WillSaveWaitUntil = "textDocument/willSaveWaitUntil";
        public const string PublishDiagnostics = "textDocument/publishDiagnostics";
        public const string FoldingRange = "textDocument/foldingRange";
        [Obsolete(Constants.Proposal)] public const string PrepareCallHierarchy = "textDocument/prepareCallHierarchy";
        [Obsolete(Constants.Proposal)] public const string CallHierarchyIncoming = "callHierarchy/incomingCalls";
        [Obsolete(Constants.Proposal)] public const string CallHierarchyOutgoing = "callHierarchy/outgoingCalls";
        [Obsolete(Constants.Proposal)] public const string SemanticTokensRegistration = "textDocument/semanticTokens";
        [Obsolete(Constants.Proposal)] public const string SemanticTokensFull = "textDocument/semanticTokens/full";
        [Obsolete(Constants.Proposal)] public const string SemanticTokensFullDelta = "textDocument/semanticTokens/full/delta";
        [Obsolete(Constants.Proposal)] public const string SemanticTokensRange = "textDocument/semanticTokens/range";
        [Obsolete(Constants.Proposal)] public const string Moniker = "textDocument/moniker";
        [Obsolete(Constants.Proposal)] public const string OnTypeRename = "textDocument/onTypeRename";
    }
}
