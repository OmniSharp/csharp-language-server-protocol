using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Delete file operation
    /// </summary>
    public class DeleteFile : IFile
    {
        /// <summary>
        /// A delete
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Delete;
        /// <summary>
        /// The file to delete.
        /// </summary>
        public DocumentUri Uri { get; set; }
        /// <summary>
        /// Delete Options.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public DeleteFileOptions Options { get; set; }
    }
}
