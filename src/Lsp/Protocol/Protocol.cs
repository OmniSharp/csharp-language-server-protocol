using Lsp.Models;

namespace Lsp.Methods
{
    public static class Protocol
    {
        public static Notification CancelRequest = new Notification<CancelParams>("$/cancelRequest");
        public static Request Initialize = new Request<InitializeParams, InitializeResult, InitializeError>("initialize");
        public static Notification Initialized = new Notification("initialized");
        public static Notification Shutdown = new Notification("shutdown");
        public static Notification Exit = new Notification("exit");
        public static Notification ShowMessage = new Notification<ShowMessageParams>("window/showMessage");
        public static Request ShowMessageRequest = new Request<ShowMessageRequestParams, MessageActionItem>("window/showMessageRequest");
        public static Notification LogMessage = new Notification<LogMessageParams>("window/logMessage");
        public static Notification Telemetry = new Notification<object>("telemetry/event");
        public static Request RegisterCapability = new Request<RegistrationParams, object>("client/registerCapability");
        public static Request UnregisterCapability = new Request<UnregistrationParams, object>("client/unregisterCapability");
        public static DynamicNotification DidChangeConfiguration = new DynamicNotification<DidChangeConfigurationParams, object>("workspace/didChangeConfiguration");
        public static DynamicNotification DidOpenTextDocument = new DynamicNotification<DidOpenTextDocumentParams, TextDocumentRegistrationOptions>("textDocument/didOpen");
        public static DynamicNotification DidChangeTextDocument = new DynamicNotification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions>("textDocument/didChange");
        public static DynamicNotification WillSaveTextDocument = new DynamicNotification<WillSaveTextDocumentParams, TextDocumentRegistrationOptions>("textDocument/willSave");
        public static DynamicRequest WillSaveWaitUntilTextDocument = new DynamicRequest<WillSaveTextDocumentParams, TextDocumentRegistrationOptions>("textDocument/willSaveWaitUntil");
        public static DynamicNotification DidSaveTextDocument = new DynamicNotification<DidSaveTextDocumentParams, TextDocumentSaveRegistrationOptions>("textDocument/didSave");
        public static DynamicNotification DidCloseTextDocument = new DynamicNotification<DidCloseTextDocumentParams, TextDocumentRegistrationOptions>("textDocument/didClose");
        public static DynamicNotification DidChangeWatchedFiles = new DynamicNotification<DidChangeWatchedFilesParams, object>("workspace/didChangeWatchedFiles");
        public static Notification PublishDiagnostics = new Notification<PublishDiagnosticsParams>("textDocument/publishDiagnostics");
        public static DynamicRequest Completion = new DynamicRequest<TextDocumentPositionParams, CompletionList, CompletionRegistrationOptions>("textDocument/completion");
        public static Request CompletionResolve = new Request<CompletionItem, CompletionItem>("completionItem/resolve");
        public static DynamicRequest Hover = new DynamicRequest<TextDocumentPositionParams, Hover, TextDocumentRegistrationOptions>("textDocument/hover");
        public static DynamicRequest SignatureHelp = new DynamicRequest<TextDocumentPositionParams, SignatureHelp, SignatureHelpRegistrationOptions>("textDocument/signatureHelp");
        public static DynamicRequest GotoDefinition = new DynamicRequest<TextDocumentPositionParams, LocationOrLocations, TextDocumentRegistrationOptions>("textDocument/definition");
        public static DynamicRequest FindReferences = new DynamicRequest<ReferenceParams, LocationContainer, TextDocumentRegistrationOptions>("textDocument/references");
        public static DynamicRequest DocumentHighlight = new DynamicRequest<TextDocumentPositionParams, DocumentHighlightContainer, TextDocumentRegistrationOptions>("textDocument/documentHighlight");
        public static DynamicRequest DocumentSymbols = new DynamicRequest<DocumentSymbolParams, SymbolInformationContainer, TextDocumentRegistrationOptions>("textDocument/documentSymbol");
        public static Request WorkspaceSymbols = new Request<WorkspaceSymbolParams, SymbolInformationContainer>("workspace/symbol");
        public static DynamicRequest CodeAction = new DynamicRequest<CodeActionParams, CommandContainer, TextDocumentRegistrationOptions>("textDocument/codeAction");
        public static DynamicRequest CodeLens = new DynamicRequest<CodeLensParams, CodeLensContainer, CodeLensRegistrationOptions>("textDocument/codeLens");
        public static Request CodeLensResolve = new Request<CodeLens, CodeLens>("codeLens/resolve");
        public static DynamicRequest DocumentLink = new DynamicRequest<DocumentLinkParams, DocumentLink, DocumentLinkRegistrationOptions>("textDocument/documentLink");
        public static Request DocumentLinkResolve = new Request<DocumentLink, DocumentLink>("documentLink/resolve");
        public static DynamicRequest DocumentFormat = new DynamicRequest<DocumentFormattingParams, TextEditContainer, TextDocumentRegistrationOptions>("textDocument/formatting");
        public static DynamicRequest OnTypeFormat = new DynamicRequest<DocumentOnTypeFormattingParams , TextEditContainer, DocumentOnTypeFormattingRegistrationOptions >("textDocument/onTypeFormatting");
        public static DynamicRequest Rename = new DynamicRequest<RenameParams , WorkspaceEdit, TextDocumentRegistrationOptions >("textDocument/onTypeFormatting");
        public static DynamicRequest ExecuteCommand = new DynamicRequest<ExecuteCommandParams , object, ExecuteCommandRegistrationOptions >("workspace/executeCommand");
        public static Request ApplyEdit = new Request<ApplyWorkspaceEditParams , ApplyWorkspaceEditResponse>("workspace/applyEdit");
    }
}