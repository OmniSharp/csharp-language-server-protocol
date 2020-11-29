using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.DidChangeWorkspaceFolders, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        public partial record DidChangeWorkspaceFoldersParams : IRequest
        {
            /// <summary>
            /// The actual workspace folder change event.
            /// </summary>
            public WorkspaceFoldersChangeEvent Event { get; init; }
        }

        /// <summary>
        /// The workspace folder change event.
        /// </summary>
        public partial record WorkspaceFoldersChangeEvent
        {
            /// <summary>
            /// The array of added workspace folders
            /// </summary>
            public Container<WorkspaceFolder> Added { get; init; } = new Container<WorkspaceFolder>();

            /// <summary>
            /// The array of the removed workspace folders
            /// </summary>
            public Container<WorkspaceFolder> Removed { get; init; } = new Container<WorkspaceFolder>();
        }
    }
}
