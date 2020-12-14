using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IFile
    {
        ResourceOperationKind Kind { get; }

        /// <summary>
        /// An optional annotation describing the operation.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        ChangeAnnotationIdentifier? AnnotationId { get; init; }
    }
}
