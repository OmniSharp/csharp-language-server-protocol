using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.DidChangeWorkspaceFolders, Direction.ClientToServer)]
    public class DidChangeWorkspaceFoldersParams : IRequest
    {
        /// <summary>
        /// The actual workspace folder change event.
        /// </summary>
        public WorkspaceFoldersChangeEvent Event { get; set; }
    }
}
