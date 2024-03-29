using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Rename file operation
    /// </summary>
    public record RenameFile : IFile
    {
        /// <summary>
        /// A rename
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Rename;

        /// <summary>
        /// The old (existing) location.
        /// </summary>
        public DocumentUri OldUri { get; init; } = null!;

        /// <summary>
        /// The new location.
        /// </summary>
        public DocumentUri NewUri { get; init; } = null!;

        /// <summary>
        /// Rename options.
        /// </summary>
        [Optional]
        public RenameFileOptions? Options { get; init; }

        /// <summary>
        /// An optional annotation identifier describing the operation.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public ChangeAnnotationIdentifier? AnnotationId { get; init; }
    }
}
