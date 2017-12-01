using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidChangeWatchedFilesParams
    {
        /// <summary>
        ///  The actual file events.
        /// </summary>
        public Container<FileEvent> Changes { get; set; }
    }
}
