using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class WorkspaceServerCapabilities
    {
        /// <summary>
        /// The server supports workspace folder.
        ///
        /// Since 3.6.0
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public WorkspaceFolderOptions WorkspaceFolders { get; set; }
    }
}
