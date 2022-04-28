using System.Diagnostics;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.DidChangeWorkspaceFolders, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DidChangeWorkspaceFolderRegistrationOptions))]
        public partial record DidChangeWorkspaceFoldersParams : IRequest
        {
            /// <summary>
            /// The actual workspace folder change event.
            /// </summary>
            public WorkspaceFoldersChangeEvent Event { get; init; } = null!;
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

        [Parallel]
        [Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceFolders"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))
        ]
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
            public DocumentUri Uri { get; init; } = null!;

            /// <summary>
            /// The name of the workspace folder. Defaults to the uri's basename.
            /// </summary>
            public string Name { get; init; } = null!;

            private string DebuggerDisplay => $"{Name} ({Uri})";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.WorkspaceFolders))]
        [RegistrationName(WorkspaceNames.DidChangeWorkspaceFolders)]
        public partial class DidChangeWorkspaceFolderRegistrationOptions
        {
            /// <summary>
            /// The server has support for workspace folders
            /// </summary>
            [Optional]
            public bool Supported { get; set; }

            /// <summary>
            /// Whether the server wants to receive workspace folder
            /// change notifications.
            ///
            /// If a strings is provided the string is treated as a ID
            /// under which the notification is registed on the client
            /// side. The ID can be used to unregister for these events
            /// using the `client/unregisterCapability` request.
            /// </summary>
            [Optional]
            public BooleanString ChangeNotifications { get; set; }
        }
    }
}
