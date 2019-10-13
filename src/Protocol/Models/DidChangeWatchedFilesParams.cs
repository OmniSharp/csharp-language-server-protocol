using MediatR;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidChangeWatchedFilesParams : IRequest
    {
        /// <summary>
        ///  The actual file events.
        /// </summary>
        public Container<FileEvent> Changes { get; set; }
    }
}
