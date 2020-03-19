using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidChangeWatchedFilesRegistrationOptions
    {
        /// <summary>
        /// The watchers to register.
        /// </summary>
        [Optional]
        public Container<FileSystemWatcher> Watchers { get; set; }
    }
}
