using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Delete file operation
    /// </summary>
    public record DeleteFile : IFile
    {
        /// <summary>
        /// A delete
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Delete;

        /// <summary>
        /// The file to delete.
        /// </summary>
        public DocumentUri Uri { get; init; } = null!;

        /// <summary>
        /// Delete options.
        /// </summary>
        [Optional]
        public DeleteFileOptions? Options { get; init; }

        /// <summary>
        /// An optional annotation identifier describing the operation.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public ChangeAnnotationIdentifier? AnnotationId { get; init; }
    }
}
