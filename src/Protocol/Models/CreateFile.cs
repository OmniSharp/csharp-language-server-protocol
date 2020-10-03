using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Create file operation
    /// </summary>
    public class CreateFile : IFile
    {
        /// <summary>
        /// A create
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Create;

        /// <summary>
        /// The resource to create.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// Additional Options
        /// </summary>
        [Optional]
        public CreateFileOptions? Options { get; set; }
    }
}
