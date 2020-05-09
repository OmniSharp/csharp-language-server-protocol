using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WorkspaceNames.ApplyEdit)]
    public class ApplyWorkspaceEditParams : IRequest<ApplyWorkspaceEditResponse>
    {
        /// <summary>
        /// An optional label of the workspace edit. This label is
        /// presented in the user interface for example on an undo
        /// stack to undo the workspace edit.
        /// </summary>
        [Optional]
        public string Label { get; set; }
        /// <summary>
        /// The edits to apply.
        /// </summary>
        public WorkspaceEdit Edit { get; set; }
    }
}
