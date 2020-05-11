using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceEditCapability
    {
        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool DocumentChanges { get; set; }
        /// <summary>
		/// The resource operations the client supports. Clients should at least
		/// support 'create', 'rename' and 'delete' files and folders.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ResourceOperationKind[] ResourceOperations { get; set; }
        /// <summary>
		/// The failure handling strategy of a client if applying the workspace edit
		/// fails.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public FailureHandlingKind? FailureHandling { get; set; }
    }
}
