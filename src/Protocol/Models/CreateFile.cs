using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Create file operation
    /// </summary>
    public record CreateFile : IFile
    {
        /// <summary>
        /// A create
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Create;

        /// <summary>
        /// The resource to create.
        /// </summary>
        public DocumentUri Uri { get; init; }

        /// <summary>
        /// Additional Options
        /// </summary>
        [Optional]
        public CreateFileOptions? Options { get; init; }
    }
}
