using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
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
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "SymbolInformation"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WorkspaceSymbolRegistrationOptions)), Capability(typeof(WorkspaceSymbolCapability))]
        public partial record SymbolInformationParams : IPartialItemsRequest<Container<SymbolInformation>?, SymbolInformation>, IWorkDoneProgressParams
        {
            /// <summary>
            /// A non-empty query string
            /// </summary>
            public string Query { get; init; }
        }

        /// <summary>
        /// The parameters of a Workspace Symbol Request.
        /// </summary>
        [Parallel]
        [Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceSymbols"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WorkspaceSymbolRegistrationOptions)), Capability(typeof(WorkspaceSymbolCapability))]
        public partial record WorkspaceSymbolParams : IPartialItemsRequest<Container<WorkspaceSymbol>?, WorkspaceSymbol>, IWorkDoneProgressParams
        {
            /// <summary>
            /// A non-empty query string
            /// </summary>
            public string Query { get; init; }
        }

        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial record SymbolInformation
        {
            /// <summary>
            /// The name of this symbol.
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// The kind of this symbol.
            /// </summary>
            public SymbolKind Kind { get; init; }

            /// <summary>
            /// Tags for this completion item.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public Container<SymbolTag>? Tags { get; init; }

            /// <summary>
            /// Indicates if this item is deprecated.
            /// </summary>
            [Optional]
            public bool Deprecated { get; init; }

            /// <summary>
            /// The location of this symbol.
            /// </summary>
            public Location Location { get; init; }

            /// <summary>
            /// The name of the symbol containing this symbol.
            /// </summary>
            [Optional]
            public string? ContainerName { get; init; }

            private string DebuggerDisplay => $"[{Kind}@{Location}] {Name}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        /// <summary>
        /// A special workspace symbol that supports locations without a range
        ///
        /// @since 3.17.0 - proposed state
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [Parallel]
        [Method(WorkspaceNames.WorkspaceSymbolResolve, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace", Name = "WorkspaceSymbolResolve")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [Capability(typeof(WorkspaceSymbolCapability))]
        public partial record WorkspaceSymbol : IRequest<WorkspaceSymbol>
        {
            /// <summary>
            /// The name of this symbol.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The kind of this symbol.
            /// </summary>
            public SymbolKind Kind { get; set; }

            /// <summary>
            /// Tags for this completion item.
            /// </summary>
            [Optional]
            public Container<SymbolTag>? Tags { get; set; }

            /// <summary>
            /// The name of the symbol containing this symbol. This information is for
            /// user interface purposes (e.g. to render a qualifier in the user interface
            /// if necessary). It can't be used to re-infer a hierarchy for the document
            /// symbols.
            /// </summary>
            [Optional]
            public string? ContainerName { get; set; }

            /// <summary>
            /// The location of this symbol. Whether a server is allowed to
            /// return a location without a range depends on the client
            /// capability `workspace.symbol.resolveSupport`.
            ///
            /// See also `SymbolInformation.location`.
            /// </summary>
            public LocationOrFileLocation Location { get; set; }

            /// <summary>
            /// A data entry field that is preserved on a workspace symbol between a
            /// workspace symbol request and a workspace symbol resolve request.
            /// </summary>
            public JToken? Data { get; set; }

            private string DebuggerDisplay => $"[{Kind}@{Location}] {Name}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
        [RegistrationOptionsConverter(typeof(WorkspaceSymbolRegistrationOptionsConverter))]
        [RegistrationName(WorkspaceNames.WorkspaceSymbol)]
        public partial class WorkspaceSymbolRegistrationOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a workspace symbol.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            private class WorkspaceSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<WorkspaceSymbolRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public WorkspaceSymbolRegistrationOptionsConverter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(WorkspaceSymbolRegistrationOptions source)
                {
                    return new()
                    {
                        ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(IWorkspaceSymbolResolveHandler)),
                        WorkDoneProgress = source.WorkDoneProgress,
                    };
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.Symbol))]
        public partial class WorkspaceSymbolCapability : DynamicCapability //
        {
            /// <summary>
            /// Specific capabilities for the `SymbolKind` in the `workspace/symbol` request.
            /// </summary>
            [Optional]
            public SymbolKindCapabilityOptions? SymbolKind { get; set; }

            /// <summary>
            /// The client supports tags on `SymbolInformation` and `WorkspaceSymbol`. Tags are supported on
            /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
            /// Clients supporting tags have to handle unknown tags gracefully.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public Supports<TagSupportCapabilityOptions?> TagSupport { get; set; }


            /// <summary>
            /// The client support partial workspace symbols. The client will send the
            /// request `workspaceSymbol/resolve` to the server to resolve additional
            /// properties.
            ///
            /// @since 3.17.0 - proposedState
            /// </summary>
            [Optional]
            public ResolveSupportCapabilityOptions? ResolveSupport { get; set; }
        }

        /// <summary>
        /// The client supports tags on `SymbolInformation`.Tags are supported on
        /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set tot true.
        /// Clients supporting tags have to handle unknown tags gracefully.
        ///
        /// @since 3.16.0
        /// </summary>
        public class TagSupportCapabilityOptions
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            public Container<SymbolTag> ValueSet { get; set; } = null!;
        }

        /// <summary>
        /// Specific capabilities for the `SymbolKind`.
        /// </summary>
        public class SymbolKindCapabilityOptions
        {
            /// <summary>
            /// The symbol kind values the client supports. When this
            /// property exists the client also guarantees that it will
            /// handle values outside its set gracefully and falls back
            /// to a default value when unknown.
            ///
            /// If this property is not present the client only supports
            /// the symbol kinds from `File` to `Array` as defined in
            /// the initial version of the protocol.
            /// </summary>
            [Optional]
            public Container<SymbolKind>? ValueSet { get; set; }
        }

        /// <summary>
        /// The client support partial workspace symbols. The client will send the
        /// request `workspaceSymbol/resolve` to the server to resolve additional
        /// properties.
        ///
        /// @since 3.17.0 - proposedState
        /// </summary>
        public partial class ResolveSupportCapabilityOptions
        {
            /// <summary>
            /// The properties that a client can resolve lazily. Usually
            /// `location.range`
            /// </summary>
            public Container<string> Properties { get; set; } = new();
        }
    }
}
