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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
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
        public partial class DocumentSymbolParams : ITextDocumentIdentifierParams,
                                                    IPartialItemsRequest<SymbolInformationOrDocumentSymbolContainer, SymbolInformationOrDocumentSymbol>,
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

        /// <summary>
        /// Represents programming constructs like variables, classes, interfaces etc. that appear in a document. Document symbols can be
        /// hierarchical and they have two ranges: one that encloses its definition and one that points to its most interesting range,
        /// e.g. the range of an identifier.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public class DocumentSymbol
        {
            /// <summary>
            /// The name of this symbol.
            /// </summary>
            public string Name { get; set; } = null!;

            /// <summary>
            /// More detail for this symbol, e.g the signature of a function. If not provided the
            /// name is used.
            /// </summary>
            [Optional]
            public string? Detail { get; set; }

            /// <summary>
            /// The kind of this symbol.
            /// </summary>
            public SymbolKind Kind { get; set; }

            /// <summary>
            /// Tags for this document symbol.
            ///
            /// @since 3.16.0 - Proposed state
            /// </summary>
            [Obsolete(Constants.Proposal)]
            [Optional]
            public Container<SymbolTag>? Tags { get; set; }

            /// <summary>
            /// Indicates if this symbol is deprecated.
            /// </summary>
            [Optional]
            public bool Deprecated { get; set; }

            /// <summary>
            /// The range enclosing this symbol not including leading/trailing whitespace but everything else
            /// like comments. This information is typically used to determine if the the clients cursor is
            /// inside the symbol to reveal in the symbol in the UI.
            /// </summary>
            public Range Range { get; set; } = null!;

            /// <summary>
            /// The range that should be selected and revealed when this symbol is being picked, e.g the name of a function.
            /// Must be contained by the the `range`.
            /// </summary>
            public Range SelectionRange { get; set; } = null!;

            /// <summary>
            /// Children of this symbol, e.g. properties of a class.
            /// </summary>
            [Optional]
            public Container<DocumentSymbol>? Children { get; set; }

            private string DebuggerDisplay => $"[{Kind}] {Name} {{ range: {Range}, selection: {SelectionRange}, detail: {Detail ?? string.Empty} }}";

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
