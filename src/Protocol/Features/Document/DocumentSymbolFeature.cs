using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.DocumentSymbol, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DocumentSymbolRegistrationOptions)), Capability(typeof(DocumentSymbolCapability))]
        public partial class DocumentSymbolParams : ITextDocumentIdentifierParams, IPartialItemsRequest<SymbolInformationOrDocumentSymbolContainer, SymbolInformationOrDocumentSymbol>,
                                            IWorkDoneProgressParams
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;
        }

        [JsonConverter(typeof(SymbolInformationOrDocumentSymbolConverter))]
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [GenerateContainer]
        public class SymbolInformationOrDocumentSymbol
        {
            public SymbolInformationOrDocumentSymbol(DocumentSymbol documentSymbol)
            {
                DocumentSymbol = documentSymbol;
                SymbolInformation = default;
            }

            public SymbolInformationOrDocumentSymbol(SymbolInformation symbolInformation)
            {
                DocumentSymbol = default;
                SymbolInformation = symbolInformation;
            }

            public bool IsDocumentSymbolInformation => SymbolInformation != null;
            public SymbolInformation? SymbolInformation { get; }

            public bool IsDocumentSymbol => DocumentSymbol != null;
            public DocumentSymbol? DocumentSymbol { get; }

            public static SymbolInformationOrDocumentSymbol Create(SymbolInformation value) => value;

            public static SymbolInformationOrDocumentSymbol Create(DocumentSymbol value) => value;

            public static implicit operator SymbolInformationOrDocumentSymbol(SymbolInformation value) => new SymbolInformationOrDocumentSymbol(value);

            public static implicit operator SymbolInformationOrDocumentSymbol(DocumentSymbol value) => new SymbolInformationOrDocumentSymbol(value);

            private string DebuggerDisplay => IsDocumentSymbol ? DocumentSymbol!.ToString() : IsDocumentSymbolInformation ? SymbolInformation!.ToString() : string.Empty;

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.DocumentSymbolProvider))]
        public partial class DocumentSymbolRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
            /// <summary>
            /// A human-readable string that is shown when multiple outlines trees
            /// are shown for the same document.
            ///
            /// @since 3.16.0 - proposed state
            /// </summary>
            [Optional]
            public string? Label { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.DocumentSymbol))]
        public partial class DocumentSymbolCapability : DynamicCapability, ConnectedCapability<IDocumentSymbolHandler>
        {
            /// <summary>
            /// Specific capabilities for the `SymbolKind` in the `textDocument/symbol` request.
            /// </summary>
            [Optional]
            public SymbolKindCapabilityOptions? SymbolKind { get; set; }

            /// <summary>
            /// Whether document symbol supports hierarchical `DocumentSymbol`s.
            /// </summary>
            [Optional]
            public bool HierarchicalDocumentSymbolSupport { get; set; }

            /// <summary>
            /// The client supports tags on `SymbolInformation`.Tags are supported on
            /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set tot true.
            /// Clients supporting tags have to handle unknown tags gracefully.
            ///
            /// @since 3.16.0
            /// </summary>
            [Obsolete(Constants.Proposal)]
            [Optional]
            public TagSupportCapabilityOptions? TagSupport { get; set; }

            /// <summary>
            /// The client supports an additional label presented in the UI when
            /// registering a document symbol provider.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public bool LabelSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
