namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidChangeWatchedFilesRegistrationOptions
    {
        /// <summary>
        /// The watchers to register.
        /// </summary>
        public Container<FileSystemWatcher> Watchers { get; set; }
    }
}