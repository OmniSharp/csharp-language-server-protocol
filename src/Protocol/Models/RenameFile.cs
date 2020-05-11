using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Rename file operation
    /// </summary>
    public class RenameFile : IFile
    {
        /// <summary>
        /// A rename
        /// </summary>
        public ResourceOperationKind Kind { get; } = ResourceOperationKind.Rename;
        /// <summary>
        /// The old (existing) location.
        /// </summary>
        public DocumentUri OldUri { get; set; }
        /// <summary>
        /// The new location.
        /// </summary>
        public DocumentUri NewUri { get; set; }
        /// <summary>
        /// Rename Options.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public RenameFileOptions Options { get; set; }
    }
}
