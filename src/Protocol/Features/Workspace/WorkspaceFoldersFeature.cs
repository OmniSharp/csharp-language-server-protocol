using System.Diagnostics;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceFolders"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial record WorkspaceFolderParams : IRequest<Container<WorkspaceFolder>?>
        {
            public static WorkspaceFolderParams Instance = new WorkspaceFolderParams();
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial record WorkspaceFolder
        {
            /// <summary>
            /// The associated URI for this workspace folder.
            /// </summary>
            public DocumentUri Uri { get; init; }

            /// <summary>
            /// The name of the workspace folder. Defaults to the uri's basename.
            /// </summary>
            public string Name { get; init; }

            private string DebuggerDisplay => $"{Name} ({Uri})";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions]
        public partial class WorkspaceFolderRegistrationOptions : IWorkspaceFolderOptions
        {
            public bool Supported { get; set; }
            public BooleanString ChangeNotifications { get; set; }
        }
    }
}
