using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.DidChangeWatchedFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DidChangeWatchedFilesRegistrationOptions)), Capability(typeof(DidChangeWatchedFilesCapability))]
        public partial record DidChangeWatchedFilesParams : IRequest<Unit>
        {
            /// <summary>
            /// The actual file events.
            /// </summary>
            public Container<FileEvent> Changes { get; init; }
        }

        [GenerateRegistrationOptions]
        public partial class DidChangeWatchedFilesRegistrationOptions
        {
            /// <summary>
            /// The watchers to register.
            /// </summary>
            [Optional]
            public Container<FileSystemWatcher>? Watchers { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.DidChangeWatchedFiles))]
        public partial class DidChangeWatchedFilesCapability : DynamicCapability
        {
            /// <summary>
            /// Whether the client has support for relative patterns
            /// or not.
            /// </summary>
            [Optional]
            public bool? RelativePatternSupport { get; init; }
        }
    }
}
