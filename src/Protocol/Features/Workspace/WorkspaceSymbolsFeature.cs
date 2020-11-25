using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        /// <summary>
        /// The parameters of a Workspace Symbol Request.
        /// </summary>
        [Parallel]
        [Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceSymbols"), GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WorkspaceSymbolRegistrationOptions)), Capability(typeof(WorkspaceSymbolCapability))]
        public partial class WorkspaceSymbolParams : IPartialItemsRequest<Container<SymbolInformation>?, SymbolInformation>, IWorkDoneProgressParams
        {
            /// <summary>
            /// A non-empty query string
            /// </summary>
            public string Query { get; set; } = null!;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
        public partial class WorkspaceSymbolRegistrationOptions : IWorkDoneProgressOptions { }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial class SymbolInformation
        {
            /// <summary>
            /// The name of this symbol.
            /// </summary>
            public string Name { get; set; } = null!;

            /// <summary>
            /// The kind of this symbol.
            /// </summary>
            public SymbolKind Kind { get; set; }

            /// <summary>
            /// Tags for this completion item.
            ///
            /// @since 3.16.0 - Proposed state
            /// </summary>
            [Obsolete(Constants.Proposal)]
            [Optional]
            public Container<SymbolTag>? Tags { get; set; }

            /// <summary>
            /// Indicates if this item is deprecated.
            /// </summary>
            [Optional]
            public bool Deprecated { get; set; }

            /// <summary>
            /// The location of this symbol.
            /// </summary>
            public Location Location { get; set; } = null!;

            /// <summary>
            /// The name of the symbol containing this symbol.
            /// </summary>
            [Optional]
            public string? ContainerName { get; set; }

            private string DebuggerDisplay => $"[{Kind}@{Location}] {Name}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }
    }
    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.Symbol))]
        public partial class WorkspaceSymbolCapability : DynamicCapability//, ConnectedCapability<IWorkspaceSymbolsHandler>
        {
            /// <summary>
            /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
            /// </summary>
            [Optional]
            public SymbolKindCapabilityOptions? SymbolKind { get; set; }

            /// <summary>
            /// The client supports tags on `SymbolInformation`.Tags are supported on
            /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
            /// Clients supporting tags have to handle unknown tags gracefully.
            ///
            /// @since 3.16.0
            /// </summary>
            [Obsolete(Constants.Proposal)]
            [Optional]
            public Supports<TagSupportCapabilityOptions?> TagSupport { get; set; }
        }
    }
}
