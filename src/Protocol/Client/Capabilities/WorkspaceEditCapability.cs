using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.WorkspaceEdit))]
    public class WorkspaceEditCapability : ICapability
    {
        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        [Optional]
        public bool DocumentChanges { get; set; }

        /// <summary>
        /// The resource operations the client supports. Clients should at least
        /// support 'create', 'rename' and 'delete' files and folders.
        /// </summary>
        [Optional]
        public Container<ResourceOperationKind>? ResourceOperations { get; set; }

        /// <summary>
        /// The failure handling strategy of a client if applying the workspace edit
        /// fails.
        /// </summary>
        [Optional]
        public FailureHandlingKind? FailureHandling { get; set; }

        /// <summary>
        /// Whether the client normalizes line endings to the client specific
        /// setting.
        /// If set to `true` the client will normalize line ending characters
        /// in a workspace edit containg to the client specific new line
        /// character.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public bool NormalizesLineEndings { get; set; }

        /// <summary>
        /// Whether the client in general supports change annotations on text edits,
        /// create file, rename file and delete file changes.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public WorkspaceEditSupportCapabilitiesChangeAnnotationSupport? ChangeAnnotationSupport { get; set; }
    }
}
