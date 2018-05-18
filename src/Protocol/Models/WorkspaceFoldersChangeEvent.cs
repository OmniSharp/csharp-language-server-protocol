namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// The workspace folder change event.
    /// </summary>
    public class WorkspaceFoldersChangeEvent
    {
        /// <summary>
        /// The array of added workspace folders
        /// </summary>
        public Container<WorkspaceFolder> Added { get; set; }

        /// <summary>
        /// The array of the removed workspace folders
        /// </summary>
        public Container<WorkspaceFolder> Removed { get; set; }
    }
}
