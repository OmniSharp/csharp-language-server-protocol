using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.DidChangeWatchedFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DidChangeWatchedFilesRegistrationOptions)), Capability(typeof(DidChangeWatchedFilesCapability))]
        public partial record DidChangeWatchedFilesParams : IRequest
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
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.DidChangeWatchedFiles))]
        public partial class DidChangeWatchedFilesCapability : DynamicCapability
        {
        }
    }
}
