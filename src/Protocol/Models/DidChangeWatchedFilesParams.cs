using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.DidChangeWatchedFiles)]
    public class DidChangeWatchedFilesParams : IRequest
    {
        /// <summary>
        ///  The actual file events.
        /// </summary>
        public Container<FileEvent> Changes { get; set; }
    }
}
