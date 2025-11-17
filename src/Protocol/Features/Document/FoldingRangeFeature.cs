using System.Diagnostics;
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
        [Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "FoldingRange")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(FoldingRangeRegistrationOptions))]
        [Capability(typeof(FoldingRangeCapability))]
        public partial record FoldingRangeRequestParam : ITextDocumentIdentifierParams, IPartialItemsRequest<Container<FoldingRange>?, FoldingRange>,
                                                         IWorkDoneProgressParams
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; } = null!;
        }

        /// <summary>
        /// Represents a folding range.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        public partial record FoldingRange
        {
            /// <summary>
            /// The zero-based line number from where the folded range starts.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            public int StartLine { get; init; }

            /// <summary>
            /// The zero-based character offset from where the folded range starts. If not defined, defaults to the length of the start line.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            [Optional]
            public int? StartCharacter { get; init; }

            /// <summary>
            /// The zero-based line number where the folded range ends.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            public int EndLine { get; init; }

            /// <summary>
            /// The zero-based character offset before the folded range ends. If not defined, defaults to the length of the end line.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            [Optional]
            public int? EndCharacter { get; init; }

            /// <summary>
            /// Describes the kind of the folding range such as `comment' or 'region'. The kind
            /// is used to categorize folding ranges and used by commands like 'Fold all comments'. See
            /// [FoldingRangeKind](#FoldingRangeKind) for an enumeration of standardized kinds.
            /// </summary>
            [Optional]
            public FoldingRangeKind? Kind { get; init; }

            /// <summary>
            /// The text that the client should show when the specified range is
            /// collapsed. If not defined or not supported by the client, a default
            /// will be chosen by the client.
            ///
            /// @since 3.17.0 - proposed
            /// </summary>
            [Optional]
            public string? CollapsedText { get; init; }

            private string DebuggerDisplay =>
                $"[start: (line: {StartLine}{( StartCharacter.HasValue ? $", char: {StartCharacter}" : string.Empty )}), end: (line: {EndLine}, char: {( EndCharacter.HasValue ? $", char: {EndCharacter}" : string.Empty )})]";

            /// <inheritdoc />
            public override string ToString()
            {
                return DebuggerDisplay;
            }
        }

        /// <summary>
        /// Enum of known range kinds
        /// </summary>
        [StringEnum]
        public readonly partial struct FoldingRangeKind
        {
            /// <summary>
            /// Folding range for a comment
            /// </summary>
            public static FoldingRangeKind Comment { get; } = new FoldingRangeKind("comment");

            /// <summary>
            /// Folding range for a imports or includes
            /// </summary>
            public static FoldingRangeKind Imports { get; } = new FoldingRangeKind("imports");

            /// <summary>
            /// Folding range for a region (e.g. `#region`)
            /// </summary>
            public static FoldingRangeKind Region { get; } = new FoldingRangeKind("region");
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.FoldingRangeProvider))]
        [RegistrationName(TextDocumentNames.FoldingRange)]
        public partial class FoldingRangeRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.FoldingRange))]
        public partial class FoldingRangeCapability : DynamicCapability
        {
            /// <summary>
            /// The maximum number of folding ranges that the client prefers to receive per document. The value serves as a
            /// hint, servers are free to follow the limit.
            /// </summary>
            /// <remarks>
            /// <see cref="uint" /> in the LSP spec
            /// </remarks>
            [Optional]
            public int? RangeLimit { get; set; }

            /// <summary>
            /// If set, the client signals that it only supports folding complete lines. If set, client will
            /// ignore specified `startCharacter` and `endCharacter` properties in a FoldingRange.
            /// </summary>
            [Optional]
            public bool LineFoldingOnly { get; set; }

            /// <summary>
            /// Specific options for the folding range kind.
            ///
            /// @since 3.17.0
            /// </summary>
            [Optional]
            public FoldingRangeCapabilityFoldingRangeKind? FoldingRangeKind { get; set; }

            /// <summary>
            /// Specific options for the folding range.
            /// @since 3.17.0
            /// </summary>
            [Optional]
            public FoldingRangeCapabilityFoldingRange? FoldingRange { get; set; }
        }

        public partial class FoldingRangeCapabilityFoldingRangeKind
        {
            /// <summary>
            /// The folding range kind values the client supports. When this
            /// property exists the client also guarantees that it will
            /// handle values outside its set gracefully and falls back
            /// to a default value when unknown.
            /// </summary>
            [Optional]
            public Container<FoldingRangeKind>? ValueSet { get; set; }
        }

        public partial class FoldingRangeCapabilityFoldingRange
        {
            /// <summary>
            /// If set, the client signals that it supports setting collapsedText on
            /// folding ranges to display custom labels instead of the default text.
            ///
            /// @since 3.17.0
            /// </summary>
            [Optional]
            public bool CollapsedText { get; set; }
        }
    }

    namespace Document
    {
    }
}
