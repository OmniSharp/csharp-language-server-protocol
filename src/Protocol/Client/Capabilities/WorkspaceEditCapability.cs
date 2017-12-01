using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceEditCapability
    {
        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        [Optional]
        public bool? DocumentChanges { get; set; }
    }
}
