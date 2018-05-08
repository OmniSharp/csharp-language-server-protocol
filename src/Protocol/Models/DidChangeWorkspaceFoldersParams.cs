using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DidChangeWorkspaceFoldersParams : IRequest
    {
        /// <summary>
        /// The actual workspace folder change event.
        /// </summary>
        public WorkspaceFoldersChangeEvent Event { get; set; }
    }
}
