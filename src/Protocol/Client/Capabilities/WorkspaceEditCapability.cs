using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceEditCapability: ICapability
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
        public ResourceOperationKind[] ResourceOperations { get; set; }
        /// <summary>
		/// The failure handling strategy of a client if applying the workspace edit
		/// fails.
        /// </summary>
        [Optional]
        public FailureHandlingKind? FailureHandling { get; set; }
    }
}
