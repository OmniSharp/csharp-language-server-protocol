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
        public DocumentUri Uri { get; init; }

        /// <summary>
        /// Delete Options.
        /// </summary>
        [Optional]
        public DeleteFileOptions? Options { get; init; }
    }
}
