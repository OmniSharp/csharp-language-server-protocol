using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public record WorkspaceEdit
    {
        /// <summary>
        /// Holds changes to existing resources.
        /// </summary>
        [Optional]
        public IDictionary<DocumentUri, IEnumerable<TextEdit>>? Changes { get; init; }

        /// <summary>
        /// Depending on the client capability
        /// `workspace.workspaceEdit.resourceOperations` document changes are either
        /// an array of `TextDocumentEdit`s to express changes to n different text
        /// documents where each text document edit addresses a specific version of
        /// a text document. Or it can contain above `TextDocumentEdit`s mixed with
        /// create, rename and delete file / folder operations.
        ///
        /// Whether a client supports versioned document edits is expressed via
        /// `WorkspaceCapability.workspaceEdit.documentChanges`.
        ///
        /// If a client neither supports `documentChanges` nor
        /// `workspace.workspaceEdit.resourceOperations` then only plain `TextEdit`s
        /// using the `changes` property are supported.
        /// </summary>
        [Optional]
        public Container<WorkspaceEditDocumentChange>? DocumentChanges { get; init; }

        /// <summary>
        /// A map of change annotations that can be referenced in
        /// `AnnotatedTextEdit`s or create, rename and delete file / folder
        /// operations.
        ///
        /// Whether clients honor this property depends on the client capability
        /// `workspace.changeAnnotationSupport`.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public IDictionary<ChangeAnnotationIdentifier, ChangeAnnotation>? ChangeAnnotations { get; init; }
    }
}
