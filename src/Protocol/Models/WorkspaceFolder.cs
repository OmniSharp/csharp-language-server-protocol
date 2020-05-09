namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        public DocumentUri Uri { get; set; }

        /// <summary>
        /// The name of the workspace folder. Defaults to the uri's basename.
        /// </summary>
        public string Name { get; set; }
    }
}
