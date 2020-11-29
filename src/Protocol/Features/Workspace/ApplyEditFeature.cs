using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.ApplyEdit, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial record ApplyWorkspaceEditParams : IRequest<ApplyWorkspaceEditResponse>
        {
            /// <summary>
            /// An optional label of the workspace edit. This label is
            /// presented in the user interface for example on an undo
            /// stack to undo the workspace edit.
            /// </summary>
            [Optional]
            public string? Label { get; init; }

            /// <summary>
            /// The edits to apply.
            /// </summary>
            public WorkspaceEdit Edit { get; init; }
        }

        public partial record ApplyWorkspaceEditResponse
        {
            /// <summary>
            /// Indicates whether the edit was applied or not.
            /// </summary>
            public bool Applied { get; init; }

            /// <summary>
            /// An optional textual description for why the edit was not applied.
            /// This may be used may be used by the server for diagnostic
            /// logging or to provide a suitable error for a request that
            /// triggered the edit.
            /// </summary>
            [Optional]
            public string? FailureReason { get; init; }

            /// <summary>
            /// Depending on the client's failure handling strategy `failedChange`
            /// might contain the index of the change that failed. This property is
            /// only available if the client signals a `failureHandlingStrategy`
            /// in its client capabilities.
            /// </summary>
            [Optional]
            public int? FailedChange { get; init; }
        }
    }
}
